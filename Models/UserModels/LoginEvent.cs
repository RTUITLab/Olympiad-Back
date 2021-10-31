using System;
using System.Collections.Generic;
using System.Text;

namespace Models.UserModels
{
    public class LoginEvent
    {
        public Guid Id { get; set; }
        public DateTimeOffset LoginTime { get; set; }
        public string UserAgent { get; set; }
        public string IP { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
