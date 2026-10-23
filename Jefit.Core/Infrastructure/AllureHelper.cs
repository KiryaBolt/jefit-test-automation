using System.Text;
using System.Text.RegularExpressions;
using Allure.Net.Commons;
using Jefit.Core.Api;

namespace Jefit.Core.Infrastructure;

public static class AllureHelper
{
    private const int MaxBodyLength = 5000;

    public static void AttachText(string name, string content) =>
        TryAttach(name, "text/plain", Encoding.UTF8.GetBytes(content), "txt");

    public static void AttachHtml(string name, string html) =>
        TryAttach(name, "text/html", Encoding.UTF8.GetBytes(html), "html");

    public static void AttachPng(string name, byte[] png) =>
        TryAttach(name, "image/png", png, "png");

    private static readonly Regex SensitiveJsonField = new(
        "\"(accessToken|refreshToken|token|email|username|password\\w*)\"\\s*:\\s*\"[^\"]*\"",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static string MaskSensitive(string text) =>
        SensitiveJsonField.Replace(text, match => $"\"{match.Groups[1].Value}\": \"***\"");

    public static void AttachApiCall(string method, string url, ApiResponse response)
    {
        var masked = MaskSensitive(response.Body);
        var body = masked.Length > MaxBodyLength
            ? masked[..MaxBodyLength] + $"\n... [обрезано, всего {masked.Length} символов]"
            : masked;

        var text = new StringBuilder()
            .AppendLine($"{method} {url}")
            .AppendLine($"Status: {response.StatusCode} {response.Raw.StatusDescription}")
            .AppendLine($"Elapsed: {response.Elapsed.TotalMilliseconds:F0} ms")
            .AppendLine($"Content-Type: {response.ContentType}")
            .AppendLine()
            .AppendLine(body)
            .ToString();

        AttachText($"HTTP {method} {url}", text);
    }

    public static void AddParameter(string name, string value)
    {
        try
        {
            AllureApi.AddTestParameter(name, value);
        }
        catch (Exception)
        {
        }
    }

    public static void WriteEnvironment(IDictionary<string, string> values)
    {
        var directory = AllureLifecycle.Instance.ResultsDirectory;
        Directory.CreateDirectory(directory);
        File.WriteAllLines(Path.Combine(directory, "environment.properties"),
            values.Select(pair => $"{pair.Key}={pair.Value}"));
    }

    public static void CopyToResults(string sourceFile)
    {
        if (!File.Exists(sourceFile)) return;
        var directory = AllureLifecycle.Instance.ResultsDirectory;
        Directory.CreateDirectory(directory);
        File.Copy(sourceFile, Path.Combine(directory, Path.GetFileName(sourceFile)), overwrite: true);
    }

    private static void TryAttach(string name, string type, byte[] content, string extension)
    {
        try
        {
            AllureApi.AddAttachment(name, type, content, extension);
        }
        catch (Exception)
        {
        }
    }
}
