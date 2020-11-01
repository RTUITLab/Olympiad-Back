using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicAPI.Requests
{
    public class ExerciseExtendedRequest : ExerciseRequest
    {
        public List<ExerciseDataRequest> InOutData { get; set; }
    }
}
