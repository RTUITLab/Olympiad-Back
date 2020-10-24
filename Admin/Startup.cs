using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Olympiad.Shared.Models.Settings;
using Olympiad.Admin.Services;
using OpenQA.Selenium;
using Microsoft.Extensions.Options;
using Olympiad.Services;
using BlazorStrap;

namespace Olympiad.Admin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AdminServiceAdminSettings>(Configuration.GetSection(nameof(AdminServiceAdminSettings)));
            services.Configure<GenerateSettings>(Configuration.GetSection(nameof(GenerateSettings)));
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("PostgresDataBase")));
            
            services.AddScoped<ChallengesService>();
            
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


            services.AddHttpClient(OlympiadWebAppHttpRequester.HttpClientName, (sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<AdminServiceAdminSettings>>();
                client.DefaultRequestHeaders.Add("Authorization", options.Value.SecurityKey);
                client.BaseAddress = new Uri(options.Value.OlympiadBaseAddress);
            });
            services.AddTransient<OlympiadWebAppHttpRequester>();
            services.AddBootstrapCss();
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
                app.UseExceptionHandler("/Error");
            }

            app.UsePathBase("/admin");
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
