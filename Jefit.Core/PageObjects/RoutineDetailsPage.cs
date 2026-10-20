using Jefit.Core.Wrappers;
using OpenQA.Selenium;

namespace Jefit.Core.PageObjects;

/// <summary>Страница плана тренировок /routines/{id}/{slug}</summary>
public class RoutineDetailsPage : BasePage
{
    public RoutineDetailsPage(IWebDriver driver) : base(driver)
    {
    }

    // На этой странице название плана оформлено как h2, а h1 нет вообще
    private Label Heading => new(_driver, By.XPath("//h1 | //h2[not(ancestor::footer)]"));

    protected override void WaitForPageLoad()
    {
        if (Heading.IsDisplayed(10)) return;

        // Переход по карточке идёт без перезагрузки страницы, и на медленном сервере содержимое
        // может не подгрузиться. Перезагрузка отдаёт страницу уже готовой.
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
                // браузер уже ответил — останавливать нечего
            }
        }

        EnsureDisplayed(Heading, "План тренировок");
    }
}