using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses
{
    public enum HiddenSolutionStatus
    {
        ErrorWhileCompile = 0,
        CompileError = 1,
        InQueue = 5,
        InProcessing = 6,
        Accepted = 8
    }
}
