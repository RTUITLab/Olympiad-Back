using Microsoft.AspNetCore.Identity;
using System;

namespace Models
{
    public class User: IdentityUser
    {   
        public string FirstName { get; set; }
        public string StudentID { get; set; }
        //public new Guid Id => Guid.Parse(base.Id);
    }
}
