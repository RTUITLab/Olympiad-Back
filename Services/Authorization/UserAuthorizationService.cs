using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympiad.Services.Authorization
{
    public class UserAuthorizationService : IUserAuthorizationService
    {
        private readonly IJwtFactory jwtFactory;
        private readonly UserManager<User> userManager;
        private readonly ILogger<UserAuthorizationService> logger;

        public UserAuthorizationService(
            IJwtFactory jwtFactory,
            UserManager<User> userManager,
            ILogger<UserAuthorizationService> logger)
        {
            this.jwtFactory = jwtFactory;
            this.userManager = userManager;
            this.logger = logger;
        }
        
        public Task<OneOf<(string accessToken, User user), IUserAuthorizationService.GetJwtForUserError>> GetJwtTokenForUser(string login, string password, bool skipPasswordCheck = false)
            => GetJwtTokenForUser(() => userManager.Users.SingleOrDefaultAsync(c => c.StudentID == login || c.UserName == login),
                async (user) =>
                {
                    if (skipPasswordCheck)
                    {
                        logger.LogWarning($"Skip password checking for user {login}");
                        return true;
                    }else
                    {
                        return await userManager.CheckPasswordAsync(user, password);
                    }
                });
        public Task<OneOf<(string accessToken, User user), IUserAuthorizationService.GetJwtForUserError>> GetJwtTokenForUser(Guid userId)
            => GetJwtTokenForUser(() => userManager.FindByIdAsync(userId.ToString()), _ => Task.FromResult(true));

        public Task<OneOf<(string accessToken, User user), IUserAuthorizationService.GetJwtForUserError>> GetJwtTokenForUser(User user)
            => GetJwtTokenForUser(() => Task.FromResult(user), _ => Task.FromResult(true));

        private async Task<OneOf<(string accessToken, User user), IUserAuthorizationService.GetJwtForUserError>> GetJwtTokenForUser(
            Func<Task<User>> findUserAction,
            Func<User, Task<bool>> checkCredentialsAction)
        {
            var targetUser = await findUserAction();

            if (targetUser == null)
            {
                return IUserAuthorizationService.GetJwtForUserError.IncorrectCredentials;
            }

            if (!await checkCredentialsAction(targetUser))
            {
                return IUserAuthorizationService.GetJwtForUserError.IncorrectCredentials;
            }

            var userRoles = await userManager.GetRolesAsync(targetUser);
            return (jwtFactory.GenerateToken(targetUser, userRoles), targetUser);
        }
    }
}
