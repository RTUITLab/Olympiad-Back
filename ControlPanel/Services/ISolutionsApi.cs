using PublicAPI.Responses.Solutions;
using PublicAPI.Responses.Solutions.Analytics;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Olympiad.ControlPanel.Services
{
    public interface ISolutionsApi
    {
        [Get("/api/solutions/analytics/forExercise/{exerciseId}")]
        Task<List<SolutionAnalyticCompactResponse>> GetSolutionsForExerciseAsync(Guid exerciseId, Guid userId);
        [Get("/api/solutions/analytics/{solutionId}")]
        Task<SolutionAnalyticsResponse> GetInfoAboutSolution(Guid solutionId);
    }
}
