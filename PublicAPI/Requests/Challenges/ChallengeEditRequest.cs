using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Requests.Challenges
{
    public class ChallengeEditRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public ChallengeAccessType? ChallengeAccessType { get; set; }
        public List<Guid> AddPersons { get; set; }
        public List<Guid> RemovePersons { get; set; }
    }
}
