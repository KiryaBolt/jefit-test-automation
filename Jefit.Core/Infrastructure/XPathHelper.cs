namespace Jefit.Core.Infrastructure;

public static class XPathHelper
{
    private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Lower = "abcdefghijklmnopqrstuvwxyz";

    public static string LowerText(string expression = ".") =>
        $"translate(normalize-space({expression}), '{Upper}', '{Lower}')";

    public static string Literal(string value)
    {
        if (!value.Contains('\'')) return $"'{value}'";
        if (!value.Contains('"')) return $"\"{value}\"";
        return "concat('" + value.Replace("'", "', \"'\", '") + "')";
    }

    public static string DeepestContaining(string text)
    {
        var literal = Literal(text.ToLowerInvariant());
        var condition = $"not(self::script) and not(self::style) and contains({LowerText()}, {literal})";
        return $"//body//*[{condition}][not(.//*[{condition}])]";
    }
}
