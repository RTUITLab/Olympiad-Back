using System;
using System.Collections.Generic;
using System.Text;

namespace Olympiad.Shared.Models
{
    public enum SolutionStatus
    {
        CompileError,
        RunTimeError,
        WrongAnswer,
        TooLongWork,
        InQueue,
        InProcessing,
        Sucessful
    }
}
