using Olympiad.Shared;
using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PublicAPI.Requests.Challenges
{
    public class UpdateChallengeInfoRequest
    {
        [Required]
        [MinLength(1), MaxLength(ChallengeLimitations.MAX_CHALLENGE_TITLE_LENGTH)]
        public string Name { get; set; }
        [MaxLength(ChallengeLimitations.MAX_CHALLENGE_DESCRIPTION_LENGTH)]
        public string Description { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public ChallengeViewMode ViewMode { get; set; }
        public ChallengeAccessType AccessType { get; set; }
    }
}
