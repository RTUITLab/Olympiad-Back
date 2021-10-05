using System.Collections.Generic;

namespace Olympiad.ControlPanel.Pages.Users.GenerateUsers
{
    internal record UserGenerateRow(string StudentID, string FirstName, string Password, List<System.Security.Claims.Claim> Claims);

}
