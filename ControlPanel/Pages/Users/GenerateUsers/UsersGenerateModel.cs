using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Olympiad.ControlPanel.Pages.Users.GenerateUsers
{
    public record UsersGenerateModel(IReadOnlyList<string> ColumnNames, IReadOnlyCollection<UserGenerateRow> UserGenerateRows);
}
