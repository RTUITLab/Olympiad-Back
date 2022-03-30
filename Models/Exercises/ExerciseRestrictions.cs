using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Exercises
{
    public class ExerciseRestrictions
    {
        public CodeRestrictions Code { get; set; }
    }
    public class CodeRestrictions
    {
        public string[] AllowedLangs { get; set; }
    }
}
