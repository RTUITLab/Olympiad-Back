using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Exercises;
using Models.Solutions;
using PublicAPI.Requests;
using Olympiad.Shared.Models;

namespace Executor
{
    interface ISolutionsBase
    {
        Task<ExerciseData[]> GetExerciseData(Guid exId);
        Task<List<Solution>> GetInQueueSolutions();
        Task SaveChanges(Guid solutionId, SolutionStatus status);
        Task SaveLog(Guid solutionId, SolutionCheckRequest solutionCheck);
    }
}