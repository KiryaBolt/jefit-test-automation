using System.Runtime.CompilerServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Jefit.Core.PageObjects;

/// <summary>Баннер "We value your privacy" -> кнопка Accept.</summary>
public static class CookieBanner
{
    private static readonly By AcceptButton =
        By.XPath("//*[self::button or @role='button'][normalize-space()='Accept']");

    // Запоминаем браузеры, где баннер уже обработан, чтобы не ждать его на каждой странице
    private static readonly ConditionalWeakTable<IWebDriver, object> HandledDrivers = new();

    public static void AcceptIfPresent(IWebDriver driver)
    {
        if (HandledDrivers.TryGetValue(driver, out _)) return;

        try
        {
            var shortWait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            shortWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
            shortWait.Until<IWebElement?>(d => d.FindElements(AcceptButton).FirstOrDefault(e => e.Displayed))?.Click();
        }
        catch (WebDriverTimeoutException)
        {
            /* баннера нет */
        }

        HandledDrivers.AddOrUpdate(driver, new object());
    }
}
