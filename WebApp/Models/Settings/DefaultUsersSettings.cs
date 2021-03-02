using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models.Settings
{
    public class DefaultUsersSettings
    {
        public bool Create { get; set; }
        public string ResetPasswordWarningText { get; set; }
        public List<DefaultUser>  Users { get; set; }
    }

    public class DefaultUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string StudentId { get; set; }
        public List<string> Roles { get; set; }
        public Dictionary<string, List<string>> Claims { get; set; }
    }
}
