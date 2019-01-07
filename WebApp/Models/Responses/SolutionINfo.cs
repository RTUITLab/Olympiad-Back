using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;
using Models.Solutions;

namespace WebApp.Models.Responses
{
    public class SolutionInfo
    {
        public Guid Id { get; set; }
        public string Language { get; set; }
        public Guid UserId { get; set; }
        public Guid ExerciseId { get; set; }
        public SolutionStatus Status { get; set; }
        public DateTime SendingTime { get; set; }
    }
}
