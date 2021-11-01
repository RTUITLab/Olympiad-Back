﻿using PublicAPI.Requests.Challenges;
using PublicAPI.Responses.Challenges;
using PublicAPI.Responses;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PublicAPI.Responses.Challenges.Analytics;

namespace Olympiad.ControlPanel.Services
{
    public interface IChallengesApi
    {
        [Get("/api/challenges/all")]
        public Task<List<ChallengeResponse>> GetAllChallengesAsync();
        [Get("/api/challenges/analytics")]
        public Task<List<ChallengeResponseWithAnalytics>> GetAllChallengesWithAnalyticsAsync();
        [Get("/api/challenges/analytics/{challengeId}")]
        public Task<ListResponse<UserChallengeResultsResponse>> GetUserResultsForChallenge(Guid challengeId, string? match, int offset, int limit);


        [Get("/api/challenges/{challengeId}")]
        public Task<ChallengeResponse> GetChallengeAsync(Guid challengeId);

        [Post("/api/challenges")]
        public Task<Guid> CreateChallengeAsync();

        [Put("/api/challenges/{challengeId}")]
        public Task<ChallengeResponse> UpdateChallengeAsync(Guid challengeId, [Body] UpdateChallengeInfoRequest request);

        [Delete("/api/challenges/{challengeId}")]
        public Task DeleteChallengeAsync(Guid challengeId);
    }
}
