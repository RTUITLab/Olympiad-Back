using PublicAPI.Requests.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Requests.Challenges
{
    public class InviteUsersRequest
    {
        public string Match { get; set; }
        public List<ClaimRequest> Claims { get; set; }
    }
}
