
using PublicAPI.Requests;
using PublicAPI.Requests.Account;
using PublicAPI.Responses;
using PublicAPI.Responses.Account;
using PublicAPI.Responses.Solutions;
using PublicAPI.Responses.Users;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Olympiad.ControlPanel.Services;
[Headers("Authorization: Bearer")]
public interface IControlPanelApiService
{

    [Get("/api/check/statistic")]
    public Task<List<SolutionsStatisticResponse>> GetSolutionsStatisticsAsync();

    [Get("/api/account")]
    public Task<ListResponseWithMatch<UserInfoResponse>> SearchUsers(string? match, int limit, int offset);
    [Get("/api/account/{userId}")]
    public Task<ApiResponse<UserInfoResponse>> GetUser(Guid userId);
    [Put("/api/account/{userId}")]
    public Task<UserInfoResponse> UpdateUserInfo(Guid userId, [Body] UpdateAccountInfoRequest body);
    [Post("/api/account/adminChangePassword/{userId}")]
    public Task<NewPasswordGeneratedResponse> GenerateNewPasswordForUser(Guid userId);
    [Post("/api/account/generate")]
    public Task GenerateNewUser([Body]GenerateUserRequest generateUserRequest);
    [Delete("/api/account/{userId}")]
    public Task<HttpResponseMessage> DeleteUser(Guid userId);

    [Get("/api/account/{userId}/claims")]
    public Task<List<ClaimResponseObject>> GetClaims(Guid userId);

    [Post("/api/account/{userId}/claims")]
    public Task<List<ClaimResponseObject>> AddClaim(Guid userId, [Body] ClaimRequest request);
    [Delete("/api/account/{userId}/claims")]
    public Task<List<ClaimResponseObject>> RemoveClaim(Guid userId, string type, string value);
    [Get("/api/account/{userId}/loginEvents")]
    public Task<List<LoginEventResponse>> GetLoginEvents(Guid userId);


    [Get("/api/auth/gettokenforuser/{userId}")]
    public Task<TokenResponse> GetTokenForUser(Guid userId);


    [Get("/api/account/claims")]
    public Task<Dictionary<string, List<string>>> GetAllClaims();
}
