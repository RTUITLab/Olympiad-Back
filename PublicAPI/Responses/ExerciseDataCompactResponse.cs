﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses
{
    public class ExerciseDataCompactResponse
    {
        public Guid Id { get; set; }
        public string InData { get; set; }
        public string OutData { get; set; }
    }
}
