using PublicAPI.Responses.Solutions;
using PublicAPI.Responses.Solutions.Analytics;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Olympiad.ControlPanel.Services
{
    public interface ISolutionsApi
    {
        [Get("/api/solutions/analytics/forExercise/{exerciseId}")]
        Task<List<SolutionAnalyticCompactResponse>> GetSolutionsForExerciseAsync(Guid exerciseId, Guid userId);
        [Get("/api/solutions/analytics/{solutionId}")]
        Task<SolutionAnalyticsResponse> GetInfoAboutSolution(Guid solutionId);
        [Get("/api/solutions/analytics/{solutionId}/buildLogs")]
        Task<List<BuildLogAnalyticsResponse>> GetBuildLogs(Guid solutionId);

        [Get("/api/solutions/analytics/{solutionId}/testGroupResults")]
        Task<List<SolutionTestGroupResulResponse>> GetTestGroupResults(Guid solutionId);
        [Get("/api/solutions/analytics/{solutionId}/checksForDataGroup")]
        Task<List<SolutionCheckResponse>> GetChecksForDataGroup(Guid solutionId, Guid testDataGroupId);



        [Get("/api/check/download/{solutionId}")]
        Task<string> GetSolutionSource(Guid solutionId);

    }
}
