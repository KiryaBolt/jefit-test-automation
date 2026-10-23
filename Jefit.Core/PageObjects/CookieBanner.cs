using System.Runtime.CompilerServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Jefit.Core.PageObjects;

public static class CookieBanner
{
    private static readonly By AcceptButton =
        By.XPath("//*[self::button or @role='button'][normalize-space()='Accept']");

    private static readonly ConditionalWeakTable<IWebDriver, object> HandledDrivers = new();

    public static void AcceptIfPresent(IWebDriver driver)
    {
        if (HandledDrivers.TryGetValue(driver, out _)) return;

        try
        {
            var shortWait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            shortWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
            var button = shortWait.Until<IWebElement?>(d => d.FindElements(AcceptButton).FirstOrDefault(e => e.Displayed));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", button);
        }
        catch (WebDriverTimeoutException)
        {
        }
        catch (WebDriverException)
        {
        }

        HandledDrivers.AddOrUpdate(driver, new object());
    }
}
