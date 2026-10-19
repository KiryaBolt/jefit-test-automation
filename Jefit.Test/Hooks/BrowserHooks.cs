using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Jefit.Test.Hooks;

/// <summary>
/// Хуки браузера. Важно: хуки с тегом ("ui") срабатывают только для UI-сценариев.
/// Хук без тега в любом [Binding]-классе выполняется для ВСЕХ сценариев проекта.
/// </summary>
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

    /// <summary>Если шаг упал — скриншот, адрес и HTML страницы прикрепляются к Allure.</summary>
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
            // браузер уже недоступен
        }
    }

    [AfterScenario(Order = 10000)]
    public void CloseBrowser() => _browser.Dispose();
}
