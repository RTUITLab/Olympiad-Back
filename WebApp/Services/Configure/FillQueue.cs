using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;
using WebApp.Configure.Models.Configure.Interfaces;
using WebApp.Models.Settings;
using WebApp.Services.Interfaces;

namespace WebApp.Services.Configure
{
    public class FillQueue : IConfigureWork
    {
        private readonly IQueueChecker queueChecker;
        private readonly ApplicationDbContext dbContext;

        public FillQueue(
            IQueueChecker queueChecker,
            ApplicationDbContext dbContext)
        {
            this.queueChecker = queueChecker;
            this.dbContext = dbContext;
        }

        public async Task Configure()
        {
            (await dbContext
                .Solutions
                .Where(s => s.Status == SolutionStatus.InQueue)
                .Select(s => s.Id)
                .ToListAsync())
                .ForEach(i => queueChecker.PutInQueue(i));
        }
    }
}
