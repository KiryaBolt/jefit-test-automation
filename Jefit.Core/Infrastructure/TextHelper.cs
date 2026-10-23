namespace Jefit.Core.Infrastructure;

public static class TextHelper
{
    public static string Normalize(string? text) =>
        new((text ?? string.Empty).ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray());

    public static string SlugToTitle(string slug) =>
        string.Join(' ', slug.Split('-', StringSplitOptions.RemoveEmptyEntries)
            .Select(word => char.ToUpperInvariant(word[0]) + word[1..]));
}
