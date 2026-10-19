namespace Jefit.Core.Infrastructure;

public static class TextHelper
{
    /// <summary>Нижний регистр, только буквы и цифры: "Push-Up (Wide)" -> "pushupwide".</summary>
    public static string Normalize(string? text) =>
        new((text ?? string.Empty).ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray());

    /// <summary>"barbell-bench-press" -> "Barbell Bench Press".</summary>
    public static string SlugToTitle(string slug) =>
        string.Join(' ', slug.Split('-', StringSplitOptions.RemoveEmptyEntries)
            .Select(word => char.ToUpperInvariant(word[0]) + word[1..]));
}
