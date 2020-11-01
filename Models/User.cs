using Microsoft.AspNetCore.Identity;
using Models.Exercises;
using Models.Solutions;
using System;
using System.Collections.Generic;

namespace Models
{
    public class User: IdentityUser<Guid>
    {   
        public string FirstName { get; set; }
        public string StudentID { get; set; }
        public List<Solution> Solutions { get; set; }
        public List<UserToGroup> UserToGroups { get; set; }
        public List<UserToExercise> UserToExercises { get; set; }
    }
}
