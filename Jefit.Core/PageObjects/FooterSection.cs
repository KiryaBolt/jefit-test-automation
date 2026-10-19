using Jefit.Core.Infrastructure;
using Jefit.Core.Wrappers;
using OpenQA.Selenium;

namespace Jefit.Core.PageObjects;

/// <summary>Подвал сайта (есть на всех страницах): Product, Resources, Legal, Support и соцсети.</summary>
public class FooterSection
{
    private readonly IWebDriver _driver;

    public FooterSection(IWebDriver driver) => _driver = driver;

    // Подвал — в конце документа, поэтому берём последнюю ссылку с таким текстом
    private Link FooterLink(string text) =>
        new(_driver, By.XPath($"(//a[normalize-space()={XPathHelper.Literal(text)}])[last()]"));

    public void ClickLink(string text) => FooterLink(text).Click();

    public string GetLinkHref(string text) => FooterLink(text).Href;
}
