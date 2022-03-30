using PublicAPI.Requests.Challenges;
using PublicAPI.Responses.Challenges;
using PublicAPI.Responses;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PublicAPI.Responses.Challenges.Analytics;
using PublicAPI.Responses.Users;
using PublicAPI.Requests;
using PublicAPI.Requests.Account;

namespace Olympiad.ControlPanel.Services;
[Headers("Authorization: Bearer")]
public interface IChallengesApi
{
    [Get("/api/challenges/all")]
    public Task<List<ChallengeResponse>> GetAllChallengesAsync();

    [Get("/api/challenges/forUser/{userId}")]
    public Task<List<ChallengeResponse>> GetChallengesForUser(Guid userId);

    [Get("/api/challenges/analytics")]
    public Task<List<ChallengeResponseWithAnalytics>> GetAllChallengesWithAnalyticsAsync();
    
    [Get("/api/challenges/analytics/{challengeId}/participants")]
    [Obsolete("Use GetUserResultsForChallenge")]
    public Task<List<string>> GetChallengeParticipantsPrivate(Guid challengeId, string? match, string? targetClaims);
    public Task<List<string>> GetChallengeParticipants(Guid challengeId, string? match, IEnumerable<ClaimRequest> targetClaims)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        return GetChallengeParticipantsPrivate(challengeId, match, ClaimRequest.PackClaimsToUrl(targetClaims));
#pragma warning restore CS0618 // Type or member is obsolete
    }

    
    [Get("/api/challenges/analytics/{challengeId}")]
    [Obsolete("Use GetUserResultsForChallenge")]
    public Task<ListResponseWithMatch<UserChallengeResultsResponse>> GetUserResultsForChallengePrivate(Guid challengeId, string? match, int offset, int limit, string? targetClaims);

    public Task<ListResponseWithMatch<UserChallengeResultsResponse>> GetUserResultsForChallenge(Guid challengeId, string? match, int offset, int limit, IEnumerable<ClaimRequest> targetClaims)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        return GetUserResultsForChallengePrivate(challengeId, match, offset, limit, ClaimRequest.PackClaimsToUrl(targetClaims));
#pragma warning restore CS0618 // Type or member is obsolete
    }


    [Get("/api/challenges/analytics/{challengeId}/info")]
    public Task<ChallengeResponseWithAnalytics> GetOneChallengeAnalycisInfo(Guid challengeId);

    [Get("/api/challenges/{challengeId}")]
    public Task<ChallengeResponse> GetChallengeAsync(Guid challengeId);

    [Post("/api/challenges")]
    public Task<Guid> CreateChallengeAsync();

    [Put("/api/challenges/{challengeId}")]
    public Task<ChallengeResponse> UpdateChallengeAsync(Guid challengeId, [Body] UpdateChallengeInfoRequest request);

    [Delete("/api/challenges/{challengeId}")]
    public Task DeleteChallengeAsync(Guid challengeId);

    [Get("/api/challenges/{challengeId}/invitations")]
    public Task<ListResponseWithMatch<UserInfoResponse>> GetInvitations(Guid challengeId, int offset, int limit);

    [Post("/api/challenges/{challengeId}/invitations")]
    public Task<int> InviteUsers(Guid challengeId, InviteUsersRequest inviteUsersRequest);
    [Post("/api/challenges/{challengeId}/invitations/{userId}")]
    public Task<bool> InviteOneUser(Guid challengeId, Guid userId);
    [Delete("/api/challenges/{challengeId}/invitations")]
    public Task<int> RemoveAllUsersFromChallenge(Guid challengeId);
    [Delete("/api/challenges/{challengeId}/invitations/{userId}")]
    public Task<bool> RemoveOneUser(Guid challengeId, Guid userId);

}
