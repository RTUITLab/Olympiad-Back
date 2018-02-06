using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models.Responces
{
    public class ExerciseInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public string TaskText { get; set; }
        public IEnumerable<Solution> Solutions { get; set; }
    }
}
