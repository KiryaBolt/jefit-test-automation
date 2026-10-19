using Jefit.Core.Wrappers;
using OpenQA.Selenium;

namespace Jefit.Core.PageObjects;

/// <summary>Страница плана тренировок /routines/{id}/{slug}</summary>
public class RoutineDetailsPage : BasePage
{
    public RoutineDetailsPage(IWebDriver driver) : base(driver)
    {
    }

    private Label Heading => new(_driver, By.TagName("h1"));

    protected override void WaitForPageLoad() => EnsureDisplayed(Heading, "План тренировок");
}
