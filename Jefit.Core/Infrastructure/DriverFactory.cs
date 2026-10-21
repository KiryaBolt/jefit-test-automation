using System.Runtime.InteropServices;
using Jefit.Core.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace Jefit.Core.Infrastructure;

/// <summary>
/// Создаёт драйвер нужного браузера: Chrome, Firefox, Edge — локально или в Selenium Grid.
/// Драйверы скачивает Selenium Manager автоматически.
/// </summary>
public static class DriverFactory
{
    public static IWebDriver CreateDriver(TestSettings settings)
    {
        // На Linux (Jenkins в Docker, GitHub Actions) браузер всегда запускается без окна
        var headless = settings.IsHeadless || RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        DriverOptions options = settings.BrowserType.ToLower() switch
        {
            "chrome" => GetChromeOptions(settings, headless),
            "firefox" => GetFirefoxOptions(settings, headless),
            "edge" => GetEdgeOptions(settings, headless),
            _ => throw new ArgumentException($"Браузер '{settings.BrowserType}' не поддерживается.")
        };

        IWebDriver driver = !string.IsNullOrWhiteSpace(settings.RemoteUrl)
            ? new RemoteWebDriver(new Uri(settings.RemoteUrl), options)
            : options switch
            {
                ChromeOptions chrome => new ChromeDriver(chrome),
                FirefoxOptions firefox => new FirefoxDriver(firefox),
                EdgeOptions edge => new EdgeDriver(edge),
                _ => throw new ArgumentException($"Браузер '{settings.BrowserType}' не поддерживается.")
            };

        if (!headless)
        {
            driver.Manage().Window.Maximize();
        }

        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(settings.ImplicitWaitTimeout);
        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(settings.PageLoadTimeout);

        return driver;
    }

    private static ChromeOptions GetChromeOptions(TestSettings settings, bool headless)
    {
        var options = new ChromeOptions();
        options.PageLoadStrategy = PageLoadStrategy.Eager;
        AddChromiumArguments(options.AddArgument, settings, headless);

        if (!string.IsNullOrWhiteSpace(settings.ChromeBinaryPath))
        {
            options.BinaryLocation = settings.ChromeBinaryPath; // Chromium в Docker-образе
        }

        return options;
    }

    private static EdgeOptions GetEdgeOptions(TestSettings settings, bool headless)
    {
        var options = new EdgeOptions();
        options.PageLoadStrategy = PageLoadStrategy.Eager;
        AddChromiumArguments(options.AddArgument, settings, headless);
        return options;
    }

    // Chrome и Edge построены на Chromium, поэтому аргументы у них общие
    private static void AddChromiumArguments(Action<string> addArgument, TestSettings settings, bool headless)
    {
        addArgument(headless ? $"--window-size={settings.WindowWidth},{settings.WindowHeight}" : "--start-maximized");
        addArgument("--lang=en-US");
        addArgument("--disable-notifications");

        if (headless)
        {
            addArgument("--headless=new");
            addArgument("--no-sandbox");            // обязательно для Docker
            addArgument("--disable-dev-shm-usage");
            addArgument($"--user-agent={settings.UserAgent}"); // без "HeadlessChrome" в User-Agent
        }
    }

    private static FirefoxOptions GetFirefoxOptions(TestSettings settings, bool headless)
    {
        var options = new FirefoxOptions();
        options.PageLoadStrategy = PageLoadStrategy.Eager;
        if (headless)
        {
            options.AddArgument($"--width={settings.WindowWidth}");
            options.AddArgument($"--height={settings.WindowHeight}");
        }
        options.SetPreference("intl.accept_languages", "en-US");
        if (headless) options.AddArgument("--headless");
        return options;
    }
}
