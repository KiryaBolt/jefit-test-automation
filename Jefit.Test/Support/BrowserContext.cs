using OpenQA.Selenium;

namespace Jefit.Test.Support;

public class BrowserContext : IDisposable
{
    private IWebDriver? _driver;

    public IWebDriver Driver => _driver ??= DriverFactory.CreateDriver(ConfigReader.Settings);

    public bool IsStarted => _driver is not null;

    public void Dispose()
    {
        _driver?.Quit();
        _driver = null;
    }
}
