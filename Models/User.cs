using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Models
{
    public class User: IdentityUser<Guid>
    {   
        public string FirstName { get; set; }
        public string StudentID { get; set; }
        public List<Solution> Solutions { get; set; }
    }
}
