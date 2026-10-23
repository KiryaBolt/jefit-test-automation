using OpenQA.Selenium;

namespace Jefit.Core.Wrappers;

public class Button : BaseElement
{
    public Button(IWebDriver driver, By locator) : base(driver, locator)
    {
    }

    public bool IsEnabled => Element.Enabled;

    public void Click()
    {
        var element = CreateWait().Until<IWebElement?>(d =>
            d.FindElements(_locator).FirstOrDefault(e => e.Displayed && e.Enabled))!;
        SafeClick(element);
    }

    public void ClickWithoutWaitingEnabled() => SafeClick(Element);

    private void SafeClick(IWebElement element)
    {
        ScrollIntoView(element);
        try
        {
            element.Click();
        }
        catch (ElementClickInterceptedException)
        {
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", element);
        }
    }
}
