using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class UserToGroup
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
    }
}
