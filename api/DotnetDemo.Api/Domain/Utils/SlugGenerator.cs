using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DotnetDemo.Domain.Utils;

public static class SlugGenerator
{
    private static readonly Regex InvalidCharsRegex = new("[^a-z0-9-]", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex MultipleHyphenRegex = new("-{2,}", RegexOptions.Compiled);

    public static string Generate(string? source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return $"artigo-{Guid.NewGuid():N}";
        }

        var normalized = source.Trim().ToLowerInvariant();
        normalized = RemoveDiacritics(normalized);
        normalized = Regex.Replace(normalized, @"\s+", "-", RegexOptions.Compiled);
        normalized = InvalidCharsRegex.Replace(normalized, string.Empty);
        normalized = MultipleHyphenRegex.Replace(normalized, "-").Trim('-');

        if (string.IsNullOrWhiteSpace(normalized))
        {
            normalized = $"artigo-{Guid.NewGuid():N}";
        }

        return normalized;
    }

    private static string RemoveDiacritics(string value)
    {
        var formD = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var ch in formD)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(ch);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}

