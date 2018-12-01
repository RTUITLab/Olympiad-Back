using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class ExerciseData
    {
        public Guid Id { get; set; }
        public Guid ExerciseId { get; set; }
        public string InData { get; set; }
        public string OutData { get; set; }
        public bool IsPublic { get; set; }
    }
}
