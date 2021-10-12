using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models.Settings
{
    public class AccountSettings
    {
        public bool IsRegisterAvailable { get; set; }
        public string EmailDomain { get; set; }
    }
}
