
using Olympiad.Shared;
using System;
using System.Linq;
using System.Security.Claims;

namespace Olympiad.ControlPanel.Extensions;
public static class ClaimsPrincipalExtensions
{
    public static bool IsTempLogin(this ClaimsPrincipal user) => 
        user.Claims
        .Any(c => c.Type == BrowserStorageJwtAuthenticationProvider.LOGIN_TYPE_CLAIM && 
            c.Value == BrowserStorageJwtAuthenticationProvider.TEMP_LOGIN);

    public static Guid Id(this ClaimsPrincipal user) => Guid.Parse(user.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);
}
