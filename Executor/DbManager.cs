using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Executor
{
    class DbManager
    {
        private readonly ApplicationDbContext dbContext;
        private readonly object locker = new object();
        public DbManager(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public ExerciseData[] GetExerciseData(Guid exId)
        {
            lock (locker)
            {
                return dbContext.TestData
                            .Where(D => D.ExerciseId == exId)
                            .ToArray();
            }
        }

        public void SaveChanges()
        {
            lock (locker)
            {
                dbContext.SaveChanges();
            }
        }

        public List<Solution> GetInQueueSolutions()
        {
            lock (locker)
            {
                return dbContext.Solutions
                    .Where(S => S.Status == SolutionStatus.InQueue)
                    .ToList();
            }
        }
    }
}
