using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Olympiad.ControlPanel.Pages.Exercises.Edit.Components;

internal class UserFileJsonOptions
{
    public static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
}
