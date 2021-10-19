using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Olympiad.Services.Authorization;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Models.Settings;

namespace WebApp.Services.Configure
{
    public class DefaultUsersTokensPrinter : IConfigureWork
    {
        private readonly IOptions<DefaultUsersSettings> options;
        private readonly IUserAuthorizationService userAuthorizationService;
        private readonly ILogger<DefaultUsersTokensPrinter> logger;

        public DefaultUsersTokensPrinter(
            IOptions<DefaultUsersSettings> options,
            IUserAuthorizationService userAuthorizationService,
            ILogger<DefaultUsersTokensPrinter> logger)
        {
            this.options = options;
            this.userAuthorizationService = userAuthorizationService;
            this.logger = logger;
        }
        public async Task Configure(CancellationToken cancellationToken)
        {
            foreach (var user in options.Value.Users)
            {
                var token = await userAuthorizationService.GetJwtTokenForUser(user.StudentId, user.Password, true);
                token.Switch(
                    success => logger.LogWarning($"DEFAULT USER {user.Email} TOKEN: {success.accessToken}"),
                    error => logger.LogError($"Can't get token for default user {user.StudentId}"));
            }
        }
    }
}
