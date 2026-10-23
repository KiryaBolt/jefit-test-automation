using Jefit.Core.Wrappers;
using OpenQA.Selenium;

namespace Jefit.Core.PageObjects;

public class RoutineDetailsPage : BasePage
{
    public RoutineDetailsPage(IWebDriver driver) : base(driver)
    {
    }

    private Label Heading => new(_driver, By.XPath("//h1 | //h2[not(ancestor::footer)]"));

    protected override void WaitForPageLoad()
    {
        if (Heading.IsDisplayed(10)) return;

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

        EnsureDisplayed(Heading, "План тренировок");
    }
}