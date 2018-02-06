using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public enum SolutionStatus
    {
        WrongAnswer,
        RunTimeError,
        CompileError,
        InQueue,
        InProcessing,
        Sucessful
    }
}
