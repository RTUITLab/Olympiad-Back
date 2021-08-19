
using PublicAPI.Responses;
using PublicAPI.Responses.Solutions;
using PublicAPI.Responses.Users;
using Refit;
using System.Net.Http.Json;

namespace Olympiad.ControlPanel.Services;
public interface IControlPanelApiService
{
    [Get("/api/check/statistic")]
    public Task<List<SolutionsStatisticResponse>> GetSolutionsStatisticsAsync();
    [Get("/api/account")]
    public Task<ListResponse<UserInfoResponse>> SearchUsers(string? match, int limit, int offset);
}
