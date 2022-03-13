using Olympiad.Shared.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Olympiad.Shared.Services
{
    public class UserSolutionsReportOptions
    {
        public bool ShowName { get; set; }
        public bool ShowChecks { get; set; }
        public ShowSolutionsMode SolutionsMode { get; set; }
        public static UserSolutionsReportOptions Default => new UserSolutionsReportOptions
        {
            ShowChecks = true,
            ShowName = true,
            SolutionsMode = ShowSolutionsMode.OnlyBest
        };
    }
}
