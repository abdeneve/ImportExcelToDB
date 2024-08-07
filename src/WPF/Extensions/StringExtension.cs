using System.Text.RegularExpressions;

namespace WPF.Extensions;

public static class StringExtension
{
    public static string ExtractDate(this string fileName)
    {
        var regex = new Regex(@"(\d{2}[-_]\d{2}[-_]\d{4})");
        var match = regex.Match(fileName);

        if (match.Success)
        {
            return match.Groups[1].Value.Replace("_", "-");
        }
        else
        {
            return string.Empty;
        }
    }
}
