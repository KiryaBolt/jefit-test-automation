using Jefit.Core.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Jefit.Core.Wrappers;

/// <summary>
/// Базовая обёртка над элементом страницы. Элемент ищется заново при каждом обращении
/// (с явным ожиданием), поэтому не бывает StaleElementReferenceException
/// на динамическом сайте (Jefit сделан на Next.js и часто перерисовывает DOM).
/// </summary>
public abstract class BaseElement
{
    protected readonly IWebDriver _driver;
    protected readonly By _locator;

    protected BaseElement(IWebDriver driver, By locator)
    {
        _driver = driver;
        _locator = locator;
    }

    /// <summary>Первый видимый элемент по локатору.</summary>
    protected IWebElement Element =>
        CreateWait().Until<IWebElement?>(d => d.FindElements(_locator).FirstOrDefault(e => e.Displayed))!;

    public string Text => Element.Text.Trim();

    public string GetAttribute(string name) => Element.GetAttribute(name) ?? string.Empty;

    public bool IsDisplayed(int timeoutSeconds = 5)
    {
        try
        {
            CreateWait(timeoutSeconds).Until(d => d.FindElements(_locator).Any(e => e.Displayed));
            return true;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    protected void ScrollIntoView(IWebElement element) =>
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", element);

    protected WebDriverWait CreateWait(int? timeoutSeconds = null)
    {
        var seconds = timeoutSeconds ?? ConfigReader.Settings.ExplicitWaitTimeout;
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(seconds));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
        return wait;
    }
}
