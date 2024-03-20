using System.Text.RegularExpressions;

namespace Executor.Utils;

internal static partial class ImageNameParser
{
    public static (string image, string tag) Parse(string imageName)
    {
        var parsed = GetImageRegex().Match(imageName);
        var name = parsed.Groups[1].Value;
        var tag = parsed.Groups[2].Value;
        return (name, tag);
    }

    [GeneratedRegex("([^:]+):([^:]+)")]
    private static partial Regex GetImageRegex();
}
