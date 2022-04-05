using System;
using System.Collections.Generic;
using System.Text;

namespace Olympiad.Shared
{
    public class ExerciseLimitations
    {
        public const int MAX_EXERCISE_TITLE_LENGTH = 100;
        /// <summary>
        /// Big body length for old base64 content, will be reduced
        /// </summary>
        public const int MAX_EXERCISE_TASK_LENGTH = 20_000;
        public const int MAX_EXERCISE_INTERNAL_DESCRIPTION_LENGTH = 20_000;
    }
}
