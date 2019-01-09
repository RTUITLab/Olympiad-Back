using Models.Links;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Exercises
{
    public class Challenge
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public ChallengeAccessType ChallengeAccessType { get; set; }

        public List<UserToChallenge> UsersToChallenges { get; set; }

        public List<Exercise> Exercises { get; set; }
    }
}
