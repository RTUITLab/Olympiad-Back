using PublicAPI.Responses.Challenges;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Olympiad.ControlPanel.Services
{
    public interface IChallengesApi
    {
        [Get("/api/challenges/all")]
        public Task<List<ChallengeResponse>> GetAllChallengesAsync();

        [Get("/api/challenges/{challengeId}")]
        public Task<ChallengeResponse> GetChallengeAsync(Guid challengeId);

        [Post("/api/challenges")]
        public Task<Guid> CreateChallengeAsync();

        [Delete("/api/challenges/{challengeId}")]
        public Task DeleteChallengeAsync(Guid challengeId);
    }
}
