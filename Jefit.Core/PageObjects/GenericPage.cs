using OpenQA.Selenium;

namespace Jefit.Core.PageObjects;

public class GenericPage : BasePage
{
    public GenericPage(IWebDriver driver) : base(driver)
    {
    }

    public static GenericPage Open(IWebDriver driver, string pathOrUrl)
    {
        NavigateTo(driver, pathOrUrl);
        return new GenericPage(driver);
    }

    public string GetBodyText() => _driver.FindElement(By.TagName("body")).Text;

    protected override void WaitForPageLoad()
    {
    }
}
