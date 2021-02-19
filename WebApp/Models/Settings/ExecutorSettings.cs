using Semver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models.Settings
{
    public class ExecutorSettings
    {
        [Required]
        public string VersionString { get; set; }
        public SemVersion Version => SemVersion.Parse(VersionString);
    }
}
