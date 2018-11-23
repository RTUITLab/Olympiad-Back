using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Models;
using WebApp.Configure.Models.Configure.Interfaces;
using WebApp.Models.Settings;

namespace WebApp.Services.Configure
{
    public class DefaultRolesConfigure : IConfigureWork
    {
        private readonly RoleManager<IdentityRole<Guid>> roleManager;
        private readonly UserManager<User> userManager;
        private readonly DefaultUserSettings options;

        public DefaultRolesConfigure(
            RoleManager<IdentityRole<Guid>> roleManager, 
            UserManager<User> userManager,
            IOptions<DefaultUserSettings> options)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.options = options.Value;
        }

        public async Task Configure()
        {
            string[] roles = { "Admin", "User", "Executor" };
            IdentityResult roleResult;

            foreach (var role in roles)
            {
                var roleExist = await roleManager.RoleExistsAsync(role);
                if (roleExist) continue;
                var identityRole = new IdentityRole<Guid> { Name = role };
                roleResult = await roleManager.CreateAsync(identityRole);
            }
            
            if (options?.Roles == null || !options.Roles.Any())
                return;

            var powerUser = await userManager.FindByEmailAsync(options.Email);
            if (powerUser == null)
                return;

            foreach (var role in options.Roles)
            {
                if (!await userManager.IsInRoleAsync(powerUser, role))
                    await userManager.AddToRoleAsync(powerUser, role);
            }
        }
    }
}
