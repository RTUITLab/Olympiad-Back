using Olympiad.Shared.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Olympiad.Admin.Services
{
    public class AdminServiceAdminSettings : AdminSettings
    {
        public string OlympiadBaseAddress { get; set; }
    }
}
