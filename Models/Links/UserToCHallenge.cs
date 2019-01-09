using Models.Exercises;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Links
{
    public class UserToChallenge
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid ChallengeId { get; set; }
        public Challenge Challenge { get; set; }
    }
}
