using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models.Responces
{
    public class ExerciseResponce
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public SolutionStatus Status { get; set; }
    }
}
