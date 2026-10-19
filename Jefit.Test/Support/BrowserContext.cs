using OpenQA.Selenium;

namespace Jefit.Test.Support;

/// <summary>
/// Один браузер на сценарий (Context Injection SpecFlow). Браузер запускается лениво — при первом обращении,
/// поэтому сценарии без UI его не открывают. Экземпляр не делится между потоками — безопасно для параллельного запуска.
/// </summary>
public class BrowserContext : IDisposable
{
    private IWebDriver? _driver;

    public IWebDriver Driver => _driver ??= DriverFactory.CreateDriver(ConfigReader.Settings);

    public bool IsStarted => _driver is not null;

    public void Dispose()
    {
        _driver?.Quit(); // Quit() сам освобождает ресурсы
        _driver = null;
    }
}
