using PublicAPI.Responses.Challenges;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Olympiad.ControlPanel.Services
{
    public interface IChallengesApi
    {
        [Get("/api/challenges")]
        public Task<List<ChallengeResponse>> GetChallengesAsync();
    }
}
