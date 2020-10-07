using System;
using System.Collections.Generic;
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
using WebApp.Helpers;
using Microsoft.AspNetCore.Identity;
using WebApp.Auth;
using Newtonsoft.Json.Serialization;
using WebApp.Services;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using WebApp.Models.Settings;
using WebApp.Services.Configure;
using WebApp.Services.Interfaces;
using WebApp.Middleware;
using Swashbuckle.AspNetCore.Swagger;
using WebApp.Services.ReCaptcha;
using Microsoft.Extensions.Hosting;
using WebApp.ViewModels.Mappings;
using WebApp.Formatting.ResponseMappers;
using Microsoft.OpenApi.Models;
using RTUITLab.AspNetCore.Configure.Configure;
using RTUITLab.AspNetCore.Configure.Invokations;
using Olympiad.Shared.Models.Settings;
using WebApp.Hubs;

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

            services.Configure<DefaultUserSettings>(Configuration.GetSection(nameof(DefaultUserSettings)));
            services.Configure<RecaptchaSettings>(Configuration.GetSection(nameof(RecaptchaSettings)));
            services.Configure<AccountSettings>(Configuration.GetSection(nameof(AccountSettings)));
            services.Configure<GenerateSettings>(Configuration.GetSection(nameof(GenerateSettings)));
            services.Configure<AdminSettings>(Configuration.GetSection(nameof(AdminSettings)));
            services.Configure<DefaultChallengeSettings>(Configuration.GetSection(nameof(DefaultChallengeSettings)));
            services.Configure<RabbitMqQueueSettings>(Configuration.GetSection(nameof(RabbitMqQueueSettings)));

            if (Configuration.GetValue<bool>("IN_MEMORY_DB"))
                services
                    .AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("local"));
            else
                services
                .AddEntityFrameworkNpgsql()
                
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("PostgresDataBase"), npgsql => npgsql.MigrationsAssembly(nameof(WebApp))));

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions)).Get<JwtIssuerOptions>();

            services.AddSingleton<IJwtFactory, JwtFactory>();

            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                jwtAppSettingOptions.SecretKey));

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions.Issuer;
                options.Audience = jwtAppSettingOptions.Audience;
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });


            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidIssuer = jwtAppSettingOptions.Issuer,

                ValidateAudience = false,
                ValidAudience = jwtAppSettingOptions.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

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
                    }
                };
            });

            // api user claim policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiUser", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
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
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                }
            );
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


            services.AddWebAppConfigure()
                .AddTransientConfigure<AutoMigrate>(0)
                .AddTransientConfigure<DefaultRolesConfigure>(1)
                // .AddTransientConfigure<FillQueue>(1) // TODO send events table
                .AddTransientConfigure<DefaultChallengeCreator>(2);

            if (Configuration.GetValue<bool>("USE_CHECKING_RESTART"))
                services.AddHostedService<RestartCheckingService>();

            services.AddSpaStaticFiles(conf => conf.RootPath = "wwwroot");
            services.AddSignalR();
            services.AddTransient<NotifyUsersService>();
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

            app.UseCors(builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

            app.UseWebAppConfigure();

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
