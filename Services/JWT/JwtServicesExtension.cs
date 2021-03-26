using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Olympiad.Services.JWT
{
    public static class JwtServicesExtension
    {
        /// <summary>
        /// Adds <see cref="IJwtFactory"/> to DI, uses <see cref="JwtIssuerOptions"/> from "configuration" to configure
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration">Service base configuration</param>
        /// <param name="options">Readed JWT options</param>
        /// <returns></returns>
        public static IServiceCollection AddJwtGenerator(this IServiceCollection services, IConfiguration configuration, out JwtIssuerOptions options)
        {
            var jwtAppSettingOptions = configuration.GetSection(nameof(JwtIssuerOptions)).Get<JwtIssuerOptions>();

            jwtAppSettingOptions.SigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAppSettingOptions.SecretKey));
            jwtAppSettingOptions.SigningCredentials = new SigningCredentials(jwtAppSettingOptions.SigningKey, SecurityAlgorithms.HmacSha256); ;

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions.Issuer;
                options.Audience = jwtAppSettingOptions.Audience;

                options.SigningKey = jwtAppSettingOptions.SigningKey;
                options.SigningCredentials = jwtAppSettingOptions.SigningCredentials;
            });
            options = jwtAppSettingOptions;

            services.AddSingleton<IJwtFactory, JwtFactory>();
            return services;
        }
    }
}
