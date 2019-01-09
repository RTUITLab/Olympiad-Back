using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicAPI.Requests
{
    public class ExerciseDataRequest
    {
        public string InData { get; set; }
        public string OutData { get; set; }
        public bool IsPublic { get; set; }
    }
}
