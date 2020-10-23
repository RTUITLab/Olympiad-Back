using Models.Links;
using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Exercises
{
    public class Challenge
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreationTime { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public ChallengeViewMode ViewMode { get; set; }
        public ChallengeAccessType ChallengeAccessType { get; set; }

        public List<UserToChallenge> UsersToChallenges { get; set; }

        public List<Exercise> Exercises { get; set; }
    }
}
