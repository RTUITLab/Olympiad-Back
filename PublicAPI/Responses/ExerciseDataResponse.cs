using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses
{
    public class ExerciseDataResponse
    {
        public Guid Id { get; set; }
        public string InData { get; set; }
        public string OutData { get; set; }
        public bool IsPublic { get; set; }
    }
}
