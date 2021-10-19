
using Olympiad.Shared;
using System.Security.Claims;

namespace Olympiad.ControlPanel.Extensions;
public static class ClaimsPrincipalExtensions
{
    public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole(RoleNames.ADMIN);
}
