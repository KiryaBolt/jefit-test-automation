using OpenQA.Selenium;

namespace Jefit.Core.Wrappers;

/// <summary>Ссылка: всё как у кнопки + адрес перехода.</summary>
public class Link : Button
{
    public Link(IWebDriver driver, By locator) : base(driver, locator)
    {
    }

    public string Href => GetAttribute("href");
}
