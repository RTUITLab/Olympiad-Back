using System.Collections.Generic;
using System.Security.Claims;

namespace Olympiad.ControlPanel.Pages.Users.GenerateUsers
{
    internal record UserGenerateResult(UserGenerateRow Raw, bool Success, string? ErrorMessage);
}
