using System.Text;
using Allure.Net.Commons;
using Jefit.Core.Api;

namespace Jefit.Core.Infrastructure;

/// <summary>Вложения и метаданные для Allure-отчёта.</summary>
public static class AllureHelper
{
    private const int MaxBodyLength = 5000;

    public static void AttachText(string name, string content) =>
        TryAttach(name, "text/plain", Encoding.UTF8.GetBytes(content), "txt");

    public static void AttachHtml(string name, string html) =>
        TryAttach(name, "text/html", Encoding.UTF8.GetBytes(html), "html");

    public static void AttachPng(string name, byte[] png) =>
        TryAttach(name, "image/png", png, "png");

    /// <summary>Каждый HTTP-запрос: метод, URL, статус, время ответа и тело (обрезанное).</summary>
    public static void AttachApiCall(string method, string url, ApiResponse response)
    {
        var body = response.Body.Length > MaxBodyLength
            ? response.Body[..MaxBodyLength] + $"\n... [обрезано, всего {response.Body.Length} символов]"
            : response.Body;

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

    /// <summary>Параметр теста (например Browser) — разделяет один сценарий в разных браузерах.</summary>
    public static void AddParameter(string name, string value)
    {
        try
        {
            AllureApi.AddTestParameter(name, value);
        }
        catch (Exception)
        {
            // Нет активного теста Allure — пропускаем
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
            // Вызов вне теста (например, в BeforeTestRun) — прикреплять некуда
        }
    }
}
