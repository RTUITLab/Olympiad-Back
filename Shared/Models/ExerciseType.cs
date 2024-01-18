using Ardalis.SmartEnum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Olympiad.Shared.Models
{
    public class ExerciseType : SmartEnum<ExerciseType>
    {
        public static readonly ExerciseType Code = new("Code", 0);
        public static readonly ExerciseType Docs = new("Docs", 1);

        private ExerciseType(string name, int value) : base(name, value)
        {
        }
        protected ExerciseType() : this("unknown", -1) { }
    }
}
