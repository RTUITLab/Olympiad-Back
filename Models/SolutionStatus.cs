using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public enum SolutionStatus
    {
        CompileError,
        RunTimeError,
        WrongAnswer,
        InQueue,
        InProcessing,
        Sucessful
    }
}
