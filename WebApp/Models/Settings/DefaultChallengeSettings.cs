using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models.Settings
{
    public class DefaultChallengeSettings
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<DefaultChallengeExercise> Exercises { get; set; }
    }
    public class DefaultChallengeExercise
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<DefaultChallengeExerciseTextData> PublicTests { get; set; }
        public List<DefaultChallengeExerciseTextData> PrivateTests { get; set; }
    }
    public class DefaultChallengeExerciseTextData
    {
        public string Input { get; set; }
        public string Output { get; set; }
    }
}
