using Jefit.Core.Infrastructure;
using Jefit.Core.Wrappers;
using OpenQA.Selenium;

namespace Jefit.Core.PageObjects;

public class FooterSection
{
    private readonly IWebDriver _driver;

    public FooterSection(IWebDriver driver) => _driver = driver;

    private Link FooterLink(string text) =>
        new(_driver, By.XPath($"(//a[normalize-space()={XPathHelper.Literal(text)}])[last()]"));

    public void ClickLink(string text) => FooterLink(text).Click();

    public string GetLinkHref(string text) => FooterLink(text).Href;
}
