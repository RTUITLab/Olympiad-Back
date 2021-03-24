using System;
using System.Collections.Generic;
using System.Text;

namespace Olympiad.Services.UserSolutionsReport
{
    public class UserSolutionsReportOptions
    {
        public bool ShowName { get; set; }
        public bool ShowChecks { get; set; }
        public ShowSolutionsMode SolutionsMode { get; set; }
    }
}
