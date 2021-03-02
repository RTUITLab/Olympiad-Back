using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Olympiad.Shared;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;
using WebApp.Models.Settings;

namespace WebApp.Services.Configure
{
    public class DefaultRolesConfigure : IConfigureWork
    {
        private readonly RoleManager<IdentityRole<Guid>> roleManager;
        private readonly UserManager<User> userManager;
        private readonly ILogger<DefaultRolesConfigure> logger;
        private readonly DefaultUsersSettings options;

        public DefaultRolesConfigure(
            RoleManager<IdentityRole<Guid>> roleManager,
            UserManager<User> userManager,
            IOptions<DefaultUsersSettings> options,
            ILogger<DefaultRolesConfigure> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
            this.options = options.Value;
        }

        public async Task Configure(CancellationToken cancellationToken)
        {
            await ApplyDefaultRoles();
            if (options.Create)
                foreach (var user in options.Users)
                {
                    await CreateUser(user);
                }
        }


        private async Task ApplyDefaultRoles()
        {
            IdentityResult roleResult;

            foreach (var role in RoleNames.AllRoles)
            {
                var roleExist = await roleManager.RoleExistsAsync(role);
                if (roleExist) continue;
                var identityRole = new IdentityRole<Guid> { Name = role };
                roleResult = await roleManager.CreateAsync(identityRole);
            }
        }

        private async Task CreateUser(DefaultUser user)
        {
            logger.LogInformation($"Creating user {user.Email}");
            var userToCreate = new User
            {
                Email = user.Email,
                UserName = user.Email,
                FirstName = user.Name,
                StudentID = user.StudentId
            };
            var createResult = await userManager.CreateAsync(userToCreate, user.Password);
            logger.LogInformation($"creating {user.Email} : {createResult.Succeeded}");
            if (createResult.Succeeded)
            {
                logger.LogInformation($"Add reset password claim to {user.Email}");
                var addClaimResult = await userManager.AddClaimAsync(userToCreate, new Claim("reset_password", "need"));
                logger.LogInformation($"Adding reset claim to {user.Email}: {addClaimResult.Succeeded}");
            }
            else
            {
                logger.LogInformation($"Can't create user {user.Email}, skip reset password claim");
            }
            if (createResult.Succeeded)
            {
                logger.LogInformation($"Apply roles to {user.Email}");
                foreach (var role in user.Roles)
                {
                    logger.LogInformation($"Apply role {role} to {user.Email}");
                    var addToRoleResult = await userManager.AddToRoleAsync(userToCreate, role);
                    logger.LogInformation($"Adding {user.Email} to role {role}: {addToRoleResult.Succeeded}");
                }
            }
        }
    }
}
