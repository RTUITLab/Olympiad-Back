using System.Collections.Generic;

namespace Olympiad.ControlPanel.Pages.Exercises.Edit.Models;

internal record JsonUserFileFormat(
    string Title,
    int Score,
    bool IsPublic,
    List<UserTestCase> Cases
);
