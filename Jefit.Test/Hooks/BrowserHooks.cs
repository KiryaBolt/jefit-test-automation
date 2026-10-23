using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Jefit.Test.Hooks;

[Binding]
public class BrowserHooks
{
    private readonly ScenarioContext _scenarioContext;
    private readonly BrowserContext _browser;

    public BrowserHooks(ScenarioContext scenarioContext, BrowserContext browser)
    {
        _scenarioContext = scenarioContext;
        _browser = browser;
    }

    [BeforeScenario("ui")]
    public void AddBrowserToReport() =>
        AllureHelper.AddParameter("Browser", ConfigReader.Settings.BrowserType);

    [AfterStep]
    public void AttachFailureArtifacts()
    {
        if (_scenarioContext.TestError is null || !_browser.IsStarted) return;

        try
        {
            var screenshot = ((ITakesScreenshot)_browser.Driver).GetScreenshot();
            AllureHelper.AttachPng("Скриншот при падении", screenshot.AsByteArray);
            AllureHelper.AttachText("URL страницы", _browser.Driver.Url);
            AllureHelper.AttachHtml("HTML страницы", _browser.Driver.PageSource);
        }
        catch (WebDriverException)
        {
        }
    }

    [AfterScenario(Order = 10000)]
    public void CloseBrowser() => _browser.Dispose();
}
