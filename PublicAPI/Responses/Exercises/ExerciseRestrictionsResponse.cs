﻿using Olympiad.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses.Exercises
{
    public class ExerciseRestrictionsResponse
    {
        public CodeRestrictionsResponse Code { get; set; }
    }

    public class CodeRestrictionsResponse
    {
        public List<ProgramRuntime> AllowedRuntimes { get; set; }
    }
}
