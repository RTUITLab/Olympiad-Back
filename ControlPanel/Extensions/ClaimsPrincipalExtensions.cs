
using Olympiad.Shared;
using System.Linq;
using System.Security.Claims;

namespace Olympiad.ControlPanel.Extensions;
public static class ClaimsPrincipalExtensions
{
    public static bool IsTempLogin(this ClaimsPrincipal user) => 
        user.Claims
        .Any(c => c.Type == BrowserStorageJwtAuthenticationProvider.LOGIN_TYPE_CLAIM && 
            c.Value == BrowserStorageJwtAuthenticationProvider.TEMP_LOGIN);
}
