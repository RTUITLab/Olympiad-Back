using PublicAPI.Responses.Users;
using System.Collections.Generic;

namespace PublicAPI.Responses.Challenges
{
    public class ChallengeExtendedResponse : ChallengeResponse
    {
        public List<UserInfoResponse> Invited { get; set; }
    }
}
