using OpenQA.Selenium;

namespace Jefit.Core.Wrappers;

public class Link : Button
{
    public Link(IWebDriver driver, By locator) : base(driver, locator)
    {
    }

    public string Href => GetAttribute("href");
}
