using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;

namespace WebApp.Services.Configure
{
    public class AutoMigrate : IConfigureWork
    {
        private readonly ApplicationDbContext dbContext;

        public AutoMigrate(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task Configure(CancellationToken cancellationToken)
        {
            var pending = await dbContext.Database.GetPendingMigrationsAsync();
            if (!pending.Any())
                return;
            await dbContext.Database.MigrateAsync();
        }
    }
}
