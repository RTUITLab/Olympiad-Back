using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Solutions
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
