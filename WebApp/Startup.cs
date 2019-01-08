using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
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
using WebApp.Configure.Models;
using WebApp.Configure.Models.Invokations;
using WebApp.Models.Settings;
using WebApp.Services.Configure;
using WebApp.Services.Interfaces;
using WebApp.Middleware;

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

            if (Configuration.GetValue<bool>("IN_MEMORY_DB"))
                services
                    .AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("local"));
            else
                services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("PostgresDataBase")));

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

            services.AddAutoMapper();
            services.AddMvc()
                .AddJsonOptions(opt => opt.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services.AddCors();
            if (Configuration.GetValue<bool>("USE_DEBUG_EMAIL_SENDER"))
                services.AddTransient<IEmailSender, DebugEmailService>();
            else
                services.AddTransient<IEmailSender, EmailService>();

            services.AddSingleton<IQueueChecker, QueueService>();


            services.AddWebAppConfigure()
                .AddTransientConfigure<AutoMigrate>()
                .AddTransientConfigure<DefaultRolesConfigure>()
                .AddTransientConfigure<FillQueue>();
            services.AddSpaStaticFiles(conf => conf.RootPath = "wwwroot");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
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
            app.UseAuthentication();
            app.UseExceptionHandlerMiddleware();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
            app.UseSpaStaticFiles();
            app.UseSpa(spa => { });
        }


    }
}
