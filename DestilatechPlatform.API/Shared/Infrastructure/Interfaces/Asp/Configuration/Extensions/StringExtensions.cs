using System.Text.RegularExpressions;

namespace DestilatechPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;

public static partial class StringExtensions
{
    [GeneratedRegex("(?<=[a-z0-9])([A-Z])", RegexOptions.Compiled)]
    private static partial Regex KebabCaseRegex();

    public static string ToKebabCase(this string text)
    {
        return string.IsNullOrEmpty(text)
            ? text
            : KebabCaseRegex().Replace(text, "-$1").Trim().ToLower();
    }
}