﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;

namespace WebApp.Services.Configure
{
    public class AutoMigrate : IConfigureWork
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<AutoMigrate> logger;

        public AutoMigrate(
            ApplicationDbContext dbContext,
            ILogger<AutoMigrate> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }
        public async Task Configure(CancellationToken cancellationToken)
        {
            await dbContext.Database.MigrateAsync();
        }
    }
}
