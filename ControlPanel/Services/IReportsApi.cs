using PublicAPI.Responses.Solutions;
using PublicAPI.Responses.Solutions.Analytics;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Olympiad.ControlPanel.Services;
[Headers("Authorization: Bearer")]
public interface IReportsApi
{
    [Get("/api/reports/challenge/{challengeId}/{studentId}")]
    Task<string> GetReportForChallenge(Guid challengeId, string studentId);
}
