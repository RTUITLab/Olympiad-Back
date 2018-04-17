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
using WebApp.Services.Interfaces;
using WebApp.Services;
using System.Collections.Concurrent;

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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("OlympDB"),
                b => b.MigrationsAssembly("WebApp")));

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
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions.Issuer,

                ValidateAudience = true,
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
            services.AddTransient<IEmailSender, EmailService>();
            services.AddSingleton(Checker(services.BuildServiceProvider()));
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

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });

            CreateRoles(serviceProvider).Wait();

        }

        public async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            string[] roles = { "Admin", "User", "Executor" };
            IdentityResult roleResult;

            foreach (var role in roles)
            {
                var roleExist = await roleManager.RoleExistsAsync(role);
                if (!roleExist)
                {
                    var identityRole = new IdentityRole<Guid> { Name = role };
                    roleResult = await roleManager.CreateAsync(identityRole);
                }
            }
            var config = Configuration.GetSection("UserSettings");
            var waitRoles = config.GetSection("Roles")
                .AsEnumerable()
                .Select(KVP => KVP.Value)
                .Where(V => V != null)
                .ToList();
            if (config["UserEmail"] == null || waitRoles.Count == 0)
                return;

            var powerUser = await userManager.FindByEmailAsync(Configuration.GetSection("UserSettings")["UserEmail"]);
            if (powerUser == null)
                return;

            foreach (var role in waitRoles)
            {
                if (!await userManager.IsInRoleAsync(powerUser, role))
                    await userManager.AddToRoleAsync(powerUser, role);
            }
        }

        public IQueueChecker Checker(IServiceProvider serviceProvider)
        {
            IQueueChecker queue = new QueueService();
            var dbManager = serviceProvider.GetRequiredService<ApplicationDbContext>();

            dbManager
                .Solutions
                .Where(S => S.Status == SolutionStatus.InQueue)
                .Select(S => S.Id)
                .ToList()
                .ForEach(Id => queue.PutInQueue(Id));

            return queue;
        }
    }
}
