﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
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