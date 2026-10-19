namespace Jefit.Core.Infrastructure;

/// <summary>Построение надёжных текстовых XPath-локаторов.</summary>
public static class XPathHelper
{
    private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Lower = "abcdefghijklmnopqrstuvwxyz";

    /// <summary>normalize-space() в нижнем регистре (в XPath 1.0 нет lower-case()).</summary>
    public static string LowerText(string expression = ".") =>
        $"translate(normalize-space({expression}), '{Upper}', '{Lower}')";

    /// <summary>Безопасно подставляет строку в XPath, даже если в ней есть апостроф.</summary>
    public static string Literal(string value)
    {
        if (!value.Contains('\'')) return $"'{value}'";
        if (!value.Contains('"')) return $"\"{value}\"";
        return "concat('" + value.Replace("'", "', \"'\", '") + "')";
    }

    /// <summary>
    /// Самый глубокий элемент страницы, содержащий текст (без учёта регистра).
    /// Работает, даже если текст разбит на несколько &lt;span&gt;. Скрипты не учитываются:
    /// в Next.js они содержат копию всего текста страницы.
    /// </summary>
    public static string DeepestContaining(string text)
    {
        var literal = Literal(text.ToLowerInvariant());
        var condition = $"not(self::script) and not(self::style) and contains({LowerText()}, {literal})";
        return $"//body//*[{condition}][not(.//*[{condition}])]";
    }
}
