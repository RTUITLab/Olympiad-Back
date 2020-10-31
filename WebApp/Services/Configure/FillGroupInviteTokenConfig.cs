using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApp.Services.Configure
{
    public class FillGroupInviteTokenConfig : IConfigureWork
    {
        private readonly ApplicationDbContext dbContext;
        private readonly GroupsService groupsService;
        private readonly ILogger<FillGroupInviteTokenConfig> logger;

        public FillGroupInviteTokenConfig(
            ApplicationDbContext dbContext,
            GroupsService groupsService,
            ILogger<FillGroupInviteTokenConfig> logger)
        {
            this.dbContext = dbContext;
            this.groupsService = groupsService;
            this.logger = logger;
        }
        public async Task Configure(CancellationToken cancellationToken)
        {
            var groupsWithoutTokens = await dbContext.Groups
                .Where(g => string.IsNullOrEmpty(g.InviteToken))
                .ToListAsync();
            logger.LogInformation($"Find {groupsWithoutTokens.Count} groups without token");
            groupsWithoutTokens.ForEach(g => g.InviteToken = groupsService.GenerateInvoteToken());
            var saved = await dbContext.SaveChangesAsync();
            logger.LogInformation($"{saved} groups updated");
        }
    }
}
