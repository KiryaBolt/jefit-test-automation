using Jefit.Core.Infrastructure;
using Jefit.Core.Wrappers;
using OpenQA.Selenium;

namespace Jefit.Core.PageObjects;

public class ExerciseDetailsPage : BasePage
{
    public ExerciseDetailsPage(IWebDriver driver) : base(driver)
    {
    }

    private Label Heading => new(_driver, By.TagName("h1"));

    public static ExerciseDetailsPage Open(IWebDriver driver, string path)
    {
        NavigateTo(driver, path);
        return new ExerciseDetailsPage(driver);
    }

    public bool HasSection(string title) =>
        new Label(_driver, By.XPath($"//*[normalize-space()={XPathHelper.Literal(title)}]")).IsDisplayed();

    protected override void WaitForPageLoad() => EnsureDisplayed(Heading, "Упражнение");
}
