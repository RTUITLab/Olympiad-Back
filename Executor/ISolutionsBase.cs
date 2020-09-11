using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Exercises;
using Models.Solutions;
using PublicAPI.Requests;
using Olympiad.Shared.Models;
using PublicAPI.Responses.Solutions;

namespace Executor
{
    interface ISolutionsBase
    {
        Task<ExerciseData[]> GetExerciseData(Guid exId);
        Task<List<Solution>> GetInQueueSolutions(string lang, int count);
        Task<List<SolutionsStatisticResponse>> GetStatistic();
        Task SaveChanges(Guid solutionId, SolutionStatus status);
        Task SaveLog(Guid solutionId, SolutionCheckRequest solutionCheck);
        Task SaveBuildLog(Guid solutionId, string log);
        Task<Solution> GetSolutionInfo(Guid solutionId);
    }
}