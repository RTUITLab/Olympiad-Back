using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Olympiad.Shared
{
    public class RoleNames
    {
        public const string USER = "User";
        public const string ADMIN = "Admin";
        public const string RESULTS_VIEWER = "ResultsViewer";
        public const string EXECUTOR = "Executor";

        public static IEnumerable<string> AllRoles => _roles.Value;

        private static Lazy<List<string>> _roles;
        static RoleNames()
        {
            _roles = new Lazy<List<string>>(() => typeof(RoleNames).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                .Select(fi => fi.GetValue(null).ToString())
                .ToList());
        }
    }
}
