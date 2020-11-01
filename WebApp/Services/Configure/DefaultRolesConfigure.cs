using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;
using WebApp.Models.Settings;

namespace WebApp.Services.Configure
{
    public class DefaultRolesConfigure : IConfigureWork
    {
        private readonly RoleManager<IdentityRole<Guid>> roleManager;
        private readonly UserManager<User> userManager;
        private readonly ILogger<DefaultRolesConfigure> logger;
        private readonly DefaultUserSettings options;

        public DefaultRolesConfigure(
            RoleManager<IdentityRole<Guid>> roleManager,
            UserManager<User> userManager,
            IOptions<DefaultUserSettings> options,
            ILogger<DefaultRolesConfigure> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
            this.options = options.Value;
        }

        public async Task Configure(CancellationToken cancellationToken)
        {
            if (options.CreateUser)
                await CreateUser(options.Email, options.Name, options.StudentId, options.Password);
            await CreateUser("user1@mycode.rtuitlab.dev", "Пользователь Первый", "ИдентификаторПервогоПользователя", "LongPassword");
            await CreateUser("user2@mycode.rtuitlab.dev", "Пользователь Второй", "ИдентификаторВторогоПользователя", "LongPassword");
            await CreateUser("user3@mycode.rtuitlab.dev", "Пользователь Третий", "ИдентификаторТретьегоПользователя", "LongPassword");
            await CreateUser("user4@mycode.rtuitlab.dev", "Пользователь Четвертый", "ИдентификаторЧетвертогоПользователя", "LongPassword");
            await CreateUser("user5@mycode.rtuitlab.dev", "Пользователь Пятый", "ИдентификаторПятогоПользователя", "LongPassword");
            await ApplyRoles();
        }

        private async Task CreateUser(string email, string name, string studentId, string password)
        {
            logger.LogInformation($"Creating user {email}");
            var createResult = await userManager.CreateAsync(new User
            {
                Email = email,
                UserName = email,
                FirstName = name,
                StudentID = studentId
            }, password);
            logger.LogInformation($"creating {email} : {createResult.Succeeded}");
        }

        private async Task ApplyRoles()
        {
            string[] roles = { "Admin", "User", "Executor", "ResultsViewer" };
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
