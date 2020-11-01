using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PublicAPI.Requests.Challenges
{
    public class ChallengeCreateEditRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public ChallengeAccessType ChallengeAccessType { get; set; }
        public Guid? GroupId { get; set; }
    }
}
