using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.ViewModels
{
    public class ExercisesViewModel
    {
        public string ExerciseName { get; set; }
        public string ExerciseTask { get; set; }
        public int Score { get; set; } = -1;
    }
}
