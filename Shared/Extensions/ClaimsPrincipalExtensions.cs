using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Olympiad.Shared.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole(RoleNames.ADMIN);
        public static bool IsResultsViewer(this ClaimsPrincipal user) => user.IsInRole(RoleNames.RESULTS_VIEWER);
    }
}
