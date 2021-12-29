using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using WebApp.Services;
using WebApp.Models.Settings;
using WebApp.Services.Configure;
using WebApp.Services.Interfaces;
using WebApp.Middleware;
using WebApp.Services.ReCaptcha;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Olympiad.Shared.Models.Settings;
using WebApp.Hubs;
using WebApp.Formatting;
using Olympiad.Services;
using Olympiad.Shared;
using System.Security.Claims;
using Olympiad.Services.Authorization;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<DefaultUsersSettings>(Configuration.GetSection(nameof(DefaultUsersSettings)));
            services.Configure<RecaptchaSettings>(Configuration.GetSection(nameof(RecaptchaSettings)));
            services.Configure<AccountSettings>(Configuration.GetSection(nameof(AccountSettings)));
            services.Configure<GenerateSettings>(Configuration.GetSection(nameof(GenerateSettings)));
            services.Configure<DefaultChallengeSettings>(Configuration.GetSection(nameof(DefaultChallengeSettings)));
            services.Configure<RabbitMqQueueSettings>(Configuration.GetSection(nameof(RabbitMqQueueSettings)));
            services.Configure<ExecutorSettings>(Configuration.GetSection(nameof(ExecutorSettings)));

            if (Configuration.GetValue<bool>("IN_MEMORY_DB"))
                services
                    .AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("local"));
            else
                services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("PostgresDataBase"), npgsql => npgsql.MigrationsAssembly(nameof(WebApp))));

            services.AddJwtGenerator(Configuration, out var jwtAppSettingOptions);
            services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidIssuer = jwtAppSettingOptions.Issuer,

                ValidateAudience = false,
                ValidAudience = jwtAppSettingOptions.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = jwtAppSettingOptions.SigningKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtAppSettingOptions.Issuer;
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
                configureOptions.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/api/hubs/solutionStatus")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }



                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        // If is executor
                        if (context.HttpContext.Request.Headers.TryGetValue("Executor-Version", out var executorVersion))
                        {
                            context.Principal.AddIdentity(new ClaimsIdentity(new Claim[] { new Claim("ExecutorVersion", executorVersion) }));
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            var executorOptions = Configuration.GetSection(nameof(ExecutorSettings)).Get<ExecutorSettings>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Executor", policy => policy
                    .RequireRole(RoleNames.EXECUTOR)
                    .RequireClaim("ExecutorVersion", executorOptions.VersionString)
                    .RequireAuthenticatedUser());
            });


            // add identity
            services.AddIdentity<User, IdentityRole<Guid>>(o =>
             {
                 // configure identity options
                 o.Password.RequireDigit = false;
                 o.Password.RequireLowercase = false;
                 o.Password.RequireUppercase = false;
                 o.Password.RequireNonAlphanumeric = false;
                 o.Password.RequiredLength = 6;
             })
                 .AddEntityFrameworkStores<ApplicationDbContext>()
                 .AddDefaultTokenProviders();

            services.AddAutoMapper(typeof(Startup));

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                    options.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Olympiad API", Version = "v1" });
            });
            services.AddCors();


            if (Configuration.GetValue<bool>("USE_DEBUG_EMAIL_SENDER"))
                services.AddTransient<IEmailSender, DebugEmailService>();
            else
            {
                services.Configure<EmailSettings>(Configuration.GetSection(nameof(EmailSettings)));
                services.AddTransient<IEmailSender, EmailService>();
            }

            if (Configuration.GetValue<bool>("USE_DEBUG_RECAPTCHA_VERIFIER"))
                services.AddTransient<IRecaptchaVerifier, DebugRecaptchaVerifier>();
            else
            {
                services.AddHttpClient(HttpRecaptchaVerifier.HttpClientName, client => client.BaseAddress = new Uri("https://www.google.com/recaptcha/api/siteverify"));
                services.AddTransient<IRecaptchaVerifier, HttpRecaptchaVerifier>();
            }

            services.AddSingleton<IQueueChecker, RabbitMQQueue>();
            services.AddTransient<UserPasswordGenerator>();

            AddConfigurationServices(services);

            if (Configuration.GetValue<bool>("USE_CHECKING_RESTART"))
                services.AddHostedService<RestartCheckingService>();

            services.AddSpaStaticFiles(conf => conf.RootPath = "wwwroot");
            services.AddSignalR();
            services.AddTransient<NotifyUsersService>();
        }

        private void AddConfigurationServices(IServiceCollection services)
        {
            List<Type> configureTypes = new List<Type>();
            void RegisterService<T>() where T : class, IConfigureWork
            {
                configureTypes.Add(typeof(T));
                services.AddScoped<T>();
            }
            RegisterService<AutoMigrate>();
            RegisterService<DefaultRolesConfigure>();
            // RegisterServic<FillQueue>() // TODO send events table
            RegisterService<DefaultChallengeCreator>();
            if (Configuration.GetValue<bool>("SHOW_DEFAULT_USER_TOKENS"))
            {
                RegisterService<DefaultUsersTokensPrinter>();
            }
            services.AddSingleton(sp => new ConfigureAllService.ServicesList(configureTypes));
            services.AddHostedService<ConfigureAllService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            var origins = Configuration.GetSection("AllowedOrigins")
                .AsEnumerable()
                .Skip(1)
                .Select(kvp => kvp.Value)
                .ToArray();
            app.UseCors(builder =>
                builder
                    .WithOrigins(origins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());

            app.UseSwagger(c => { c.RouteTemplate = "api/{documentName}/swagger.json"; });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api/v1/swagger.json", "My API V1");
                c.RoutePrefix = "api";
            });
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseExceptionHandlerMiddleware();
            app.UseEndpoints(ep =>
            {
                ep.MapControllers();
                ep.MapHub<SolutionStatusHub>("/api/hubs/solutionStatus");
            });
            app.UseSpaStaticFiles();
            app.UseSpa(spa => { });
        }


    }
}
