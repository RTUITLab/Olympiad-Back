using System;
using System.Collections.Generic;
using System.Text;

namespace Olympiad.Shared.Models
{
    public enum SolutionStatus
    {
        ErrorWhileCompile,
        CompileError,
        RunTimeError,
        WrongAnswer,
        TooLongWork,
        InQueue,
        InProcessing,
        Sucessful
    }
}
