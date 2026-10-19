namespace Jefit.Core.Configuration;

/// <summary>
/// Настройки тестов. Читаются из секции "TestSettings" файла appsettings.json.
/// Любое значение можно переопределить переменной окружения:
/// JEFIT_TestSettings__BrowserType=firefox, JEFIT_TestSettings__IsHeadless=true.
/// </summary>
public class TestSettings
{
    // ---------- UI ----------
    public string BrowserType { get; set; } = "Chrome";
    public string BaseUrl { get; set; } = "https://www.jefit.com/";
    public bool IsHeadless { get; set; } = false;

    /// <summary>
    /// Неявное ожидание. По умолчанию 0: во фреймворке используются явные ожидания (WebDriverWait),
    /// а смешивать неявные и явные ожидания Selenium не рекомендует — это даёт непредсказуемые задержки.
    /// </summary>
    public int ImplicitWaitTimeout { get; set; } = 0;

    public int ExplicitWaitTimeout { get; set; } = 15;
    public int PageLoadTimeout { get; set; } = 60;
    public int WindowWidth { get; set; } = 1920;
    public int WindowHeight { get; set; } = 1080;

    /// <summary>Адрес Selenium Grid / Selenoid. Пусто — локальный браузер.</summary>
    public string RemoteUrl { get; set; } = string.Empty;

    /// <summary>Путь к Chromium в Docker-образе Jenkins (например /usr/bin/chromium). Пусто — обычный Chrome.</summary>
    public string ChromeBinaryPath { get; set; } = string.Empty;

    // ---------- API ----------
    public string ApiBaseUrl { get; set; } = "https://www.jefit.com";
    public int ApiTimeout { get; set; } = 30;
    public int MaxResponseTimeMs { get; set; } = 5000;
    public string UserAgent { get; set; } =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0 Safari/537.36 JefitAutomation/1.0";

    // ---------- Тестовый аккаунт (тесты @auth) ----------
    public string UserEmail { get; set; } = string.Empty;
    public string UserPassword { get; set; } = string.Empty;

    // У Jefit нет публичного API: пути берутся из DevTools -> Network (см. README).
    public string AccountApiBaseUrl { get; set; } = string.Empty;
    public string AccountLoginPath { get; set; } = string.Empty;
    public string AccountRoutinesPath { get; set; } = string.Empty;
    public string MyPlansPath { get; set; } = string.Empty;

    public bool HasCredentials => !string.IsNullOrWhiteSpace(UserEmail) && !string.IsNullOrWhiteSpace(UserPassword);

    public bool IsAccountApiConfigured => !string.IsNullOrWhiteSpace(AccountLoginPath);
}
