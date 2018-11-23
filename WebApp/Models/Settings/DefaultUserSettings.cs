using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models.Settings
{
    public class DefaultUserSettings
    {
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}
