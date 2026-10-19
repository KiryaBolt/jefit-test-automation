using System.Text.RegularExpressions;
using Jefit.Core.Infrastructure;

namespace Jefit.Core.Api;

/// <summary>Общие правила распознавания ссылок на упражнения и планы (используются и в UI, и в API).</summary>
public static class CardParser
{
    public static readonly Regex ExerciseHref = new(@"/exercises/(\d+)/([\w-]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public static readonly Regex RoutineHref = new(@"/routines/(\d+)/([\w-]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public const string ExerciseAltSuffix = " Demonstration";
    public const string RoutineAltSuffix = " banner";

    /// <summary>Название берётся из alt картинки карточки ("Barbell Bench Press Demonstration"), запасной вариант — slug.</summary>
    public static string ResolveName(string? imageAlt, string slug, string altSuffix)
    {
        var name = (imageAlt ?? string.Empty).Trim();
        if (name.EndsWith(altSuffix, StringComparison.OrdinalIgnoreCase))
        {
            name = name[..^altSuffix.Length].Trim();
        }

        return string.IsNullOrWhiteSpace(name) || name.Equals("banner", StringComparison.OrdinalIgnoreCase)
            ? TextHelper.SlugToTitle(slug)
            : name;
    }
}
