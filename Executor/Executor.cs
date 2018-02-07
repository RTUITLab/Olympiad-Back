using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Executor
{
    class Executor
    {
        private ApplicationDbContext db;
        private object connectionString;

        public Executor(string dbConnectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(dbConnectionString);
            db = new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
