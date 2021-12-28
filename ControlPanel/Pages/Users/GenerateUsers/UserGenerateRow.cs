using System.Collections.Generic;

namespace Olympiad.ControlPanel.Pages.Users.GenerateUsers
{
    public record UserGenerateRow(string StudentID, string FirstName, string Password, List<System.Security.Claims.Claim> Claims);

}
