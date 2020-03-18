using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Executor.Models.Settings
{
    public class RunningSettings
    {
        [Range(1, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int WorkersPerCheckCount { get; set; }
    }
}
