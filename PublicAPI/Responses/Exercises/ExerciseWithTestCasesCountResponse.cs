﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses.Exercises
{
    public class ExerciseWithTestCasesCountResponse : ExerciseCompactResponse
    {
        public int TestCasesCount { get; set; }
    }
}
