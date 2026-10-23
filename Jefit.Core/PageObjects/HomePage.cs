using Jefit.Core.Wrappers;
using OpenQA.Selenium;

namespace Jefit.Core.PageObjects;

public class HomePage : BasePage
{
    public HomePage(IWebDriver driver) : base(driver)
    {
    }

    private Label Heading => new(_driver, By.TagName("h1"));
    private Link GetStartedLink => new(_driver, By.XPath("//a[normalize-space()='Get Started']"));

    public FooterSection Footer => new(_driver);

    public static HomePage Open(IWebDriver driver)
    {
        NavigateTo(driver, "/");
        return new HomePage(driver);
    }

    public void ClickGetStarted() => GetStartedLink.Click();

    protected override void WaitForPageLoad() => EnsureDisplayed(Heading, "Главная");
}
