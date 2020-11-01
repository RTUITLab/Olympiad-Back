using Olympiad.Shared.Models;
using PublicAPI.Responses.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses
{
    public class CompactExerciseUserResult
    {
        public int UserSum { get; set; }
        public int TotalSum { get; set; }
        public SolutionStatus Status { get; set; }
        public UserInfoResponse User { get; set; }
        public DateTimeOffset SendedTime { get; set; }
    }
}
