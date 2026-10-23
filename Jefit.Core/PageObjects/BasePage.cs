using Jefit.Core.Configuration;
using Jefit.Core.Wrappers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Jefit.Core.PageObjects;

public abstract class BasePage
{
    protected readonly IWebDriver _driver;
    protected readonly WebDriverWait _wait;

    protected BasePage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(ConfigReader.Settings.ExplicitWaitTimeout));
        _wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));

        WaitForDocumentReady();
        CookieBanner.AcceptIfPresent(driver);
        WaitForPageLoad();
    }

    protected abstract void WaitForPageLoad();

    public string GetUrl() => _driver.Url;

    public string GetPageTitle() => _driver.Title;

    public string GetMainHeading()
    {
        var heading = _wait.Until<IWebElement?>(d =>
            d.FindElements(By.CssSelector("h1")).FirstOrDefault(e => e.Displayed)
            ?? d.FindElements(By.CssSelector("h2")).FirstOrDefault(e => e.Displayed));

        return heading!.Text.Trim();
    }

    public bool WaitUntil(Func<IWebDriver, bool> condition)
    {
        try
        {
            _wait.Until(condition);
            return true;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public bool WaitForUrlContains(string part) =>
        WaitUntil(d => d.Url.Contains(part, StringComparison.OrdinalIgnoreCase));

    public bool WaitForTitleContains(string part) =>
        WaitUntil(d => d.Title.Contains(part, StringComparison.OrdinalIgnoreCase));

    protected static void NavigateTo(IWebDriver driver, string pathOrUrl)
    {
        var url = pathOrUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? pathOrUrl
            : $"{ConfigReader.Settings.BaseUrl.TrimEnd('/')}/{pathOrUrl.TrimStart('/')}";

        try
        {
            driver.Navigate().GoToUrl(url);
        }
        catch (WebDriverTimeoutException)
        {
            try
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("window.stop();");
            }
            catch (WebDriverException)
            {
            }
        }
    }
    
    protected void EnsureDisplayedWithReload(BaseElement element, string pageName, int firstWaitSeconds = 12)
    {
        if (element.IsDisplayed(firstWaitSeconds)) return;

        try
        {
            _driver.Navigate().Refresh();
        }
        catch (WebDriverTimeoutException)
        {
            try
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("window.stop();");
            }
            catch (WebDriverException)
            {
            }
        }

        EnsureDisplayed(element, pageName);
    }

    protected void EnsureDisplayed(BaseElement element, string pageName)
    {
        if (!element.IsDisplayed(ConfigReader.Settings.ExplicitWaitTimeout))
        {
            throw new InvalidOperationException($"Страница \"{pageName}\" не загрузилась. Текущий адрес: {_driver.Url}");
        }
    }

    private void WaitForDocumentReady() =>
        _wait.Until(d =>
        {
            var state = ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState")?.ToString();
            return state is "interactive" or "complete";
        });
}
