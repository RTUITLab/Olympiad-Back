using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Olympiad.ControlPanel.Pages.Users.GenerateUsers
{
    internal record UsersGenerateModel(string SourceFileName, IReadOnlyList<string> ColumnNames, IReadOnlyCollection<UserGenerateRow> UserGenerateRows);
}
