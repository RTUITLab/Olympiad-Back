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
        [MinLength(1), MaxLength(250)]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public ChallengeViewMode ViewMode { get; set; }
        public ChallengeAccessType AccessType { get; set; }
    }
}
