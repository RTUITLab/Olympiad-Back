
using PublicAPI.Requests;
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
    [Get("/api/account/{userId}")]
    public Task<ApiResponse<UserInfoResponse>> GetUser(Guid userId);
    [Put("/api/account/{userId}")]
    public Task<UserInfoResponse> UpdateUserInfo(Guid userId, [Body] UpdateAccountInfoRequest body);
    [Delete("/api/account/{userId}")]
    public Task<HttpResponseMessage> DeleteUser(Guid userId);

    [Get("/api/auth/gettokenforuser/{userId}")]
    public Task<TokenResponse> GetTokenForUser(Guid userId);
}
