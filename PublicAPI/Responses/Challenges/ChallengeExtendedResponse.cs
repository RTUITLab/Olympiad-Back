using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses.Challenges
{
    public class ChallengeExtendedResponse : ChallengeResponse
    {
        public List<UserResponse> InvitedUsers { get; set; }
    }
}
