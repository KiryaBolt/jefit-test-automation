using System.Text.RegularExpressions;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Jefit.Core.Api.Models;

namespace Jefit.Core.Api;

public static class HtmlPageParser
{
    public static PageMeta GetMeta(string html)
    {
        var document = Parse(html);
        return new PageMeta(
            Title: document.Title?.Trim() ?? string.Empty,
            Description: MetaContent(document, "name", "description"),
            OgTitle: MetaContent(document, "property", "og:title"),
            Canonical: document.QuerySelector("link[rel='canonical']")?.GetAttribute("href"),
            H1: document.QuerySelector("h1")?.TextContent.Trim());
    }

    public static IReadOnlyList<ExerciseInfo> GetExercises(string html) =>
        ParseLinks(html, CardParser.ExerciseHref, CardParser.ExerciseAltSuffix,
            (id, slug, name, url) => new ExerciseInfo(id, slug, name, url));

    public static IReadOnlyList<RoutineInfo> GetRoutines(string html) =>
        ParseLinks(html, CardParser.RoutineHref, CardParser.RoutineAltSuffix,
            (id, slug, name, url) => new RoutineInfo(id, slug, name, url));

    private static IHtmlDocument Parse(string html) => new HtmlParser().ParseDocument(html);

    private static string? MetaContent(IHtmlDocument document, string attribute, string value) =>
        document.QuerySelector($"meta[{attribute}='{value}']")?.GetAttribute("content");

    private static List<T> ParseLinks<T>(string html, Regex hrefRegex, string altSuffix,
        Func<int, string, string, string, T> create)
    {
        var result = new List<T>();
        var seenIds = new HashSet<int>();

        foreach (var link in Parse(html).QuerySelectorAll("a[href]"))
        {
            var match = hrefRegex.Match(link.GetAttribute("href") ?? string.Empty);
            if (!match.Success) continue;

            var id = int.Parse(match.Groups[1].Value);
            if (!seenIds.Add(id)) continue;

            var slug = match.Groups[2].Value;
            var alt = link.QuerySelector("img[alt]")?.GetAttribute("alt");
            result.Add(create(id, slug, CardParser.ResolveName(alt, slug, altSuffix), match.Value));
        }

        return result;
    }
}
