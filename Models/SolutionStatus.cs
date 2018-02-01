using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public enum SolutionStatus
    {
        InQueue,
        InProcessing,
        CompileError,
        RunTimeError,
        Sucessful,
        WrongAnswer
    }
}
