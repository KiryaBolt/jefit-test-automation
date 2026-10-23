namespace Jefit.Core.Configuration;

public class TestSettings
{
    public string BrowserType { get; set; } = "Chrome";
    public string BaseUrl { get; set; } = "https://www.jefit.com/";
    public bool IsHeadless { get; set; } = false;

    public int ImplicitWaitTimeout { get; set; } = 0;

    public int ExplicitWaitTimeout { get; set; } = 15;
    public int PageLoadTimeout { get; set; } = 60;
    public int WindowWidth { get; set; } = 1920;
    public int WindowHeight { get; set; } = 1080;

    public string RemoteUrl { get; set; } = string.Empty;

    public string ChromeBinaryPath { get; set; } = string.Empty;

    public string ApiBaseUrl { get; set; } = "https://www.jefit.com";
    public int ApiTimeout { get; set; } = 30;
    public int MaxResponseTimeMs { get; set; } = 5000;
    public string UserAgent { get; set; } =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0 Safari/537.36 JefitAutomation/1.0";

    public string UserEmail { get; set; } = string.Empty;
    public string UserPassword { get; set; } = string.Empty;

    public string AccountApiBaseUrl { get; set; } = string.Empty;
    public string AccountLoginPath { get; set; } = string.Empty;
    public string AccountRoutinesPath { get; set; } = string.Empty;
    public string MyPlansPath { get; set; } = string.Empty;

    public bool HasCredentials => !string.IsNullOrWhiteSpace(UserEmail) && !string.IsNullOrWhiteSpace(UserPassword);

    public bool IsAccountApiConfigured => !string.IsNullOrWhiteSpace(AccountLoginPath);
}
