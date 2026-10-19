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

    protected override void WaitForPageLoad() => EnsureDisplayed(Heading, "План тренировок");
}