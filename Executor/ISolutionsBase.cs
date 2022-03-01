using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Exercises;
using Models.Solutions;
using PublicAPI.Requests;
using Olympiad.Shared.Models;
using PublicAPI.Responses.Solutions;
using PublicAPI.Responses.ExercisesTestData;

namespace Executor
{
    interface ISolutionsBase
    {
        Task<ExerciseDataResponse[]> GetExerciseData(Guid exId);
        Task<List<Solution>> GetInQueueSolutions(string lang, int count);
        Task<List<SolutionsStatisticResponse>> GetStatistic();
        Task SaveChanges(Guid solutionId, SolutionStatus status);
        Task SaveLog(Guid solutionId, Guid testDataId, SolutionCheckRequest solutionCheck);
        Task SaveBuildLog(Guid solutionId, BuildLogRequest buildLog);
        Task<Solution> GetSolutionInfo(Guid solutionId);
    }
}