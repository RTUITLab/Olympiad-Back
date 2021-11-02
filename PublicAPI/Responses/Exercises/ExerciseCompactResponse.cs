﻿using PublicAPI.Responses.Challenges;
using System;
using System.Collections.Generic;
using PublicAPI.Responses.Solutions;

namespace PublicAPI.Responses.Exercises
{
    public class ExerciseCompactResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
    }
}
