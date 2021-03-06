using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Exercises
{
    public class ExerciseData
    {
        public Guid Id { get; set; }
        public string InData { get; set; }
        public string OutData { get; set; }
        
        public ExerciseDataGroup ExerciseDataGroup { get; set; }
        public Guid ExerciseDataGroupId { get; set; }
    }
}
