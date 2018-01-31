using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Solution
    {
        public string Language { get; set; }
        public Guid Id { get; set; }
        public string Raw { get; set; }
        public Guid UserId { get; set; }
        public Guid ExerciseId { get; set; }
    }
}
