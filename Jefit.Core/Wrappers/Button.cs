using OpenQA.Selenium;

namespace Jefit.Core.Wrappers;

public class Button : BaseElement
{
    public Button(IWebDriver driver, By locator) : base(driver, locator)
    {
    }

    public bool IsEnabled => Element.Enabled;

    /// <summary>Ждёт, пока кнопка станет видимой и активной, и нажимает её.</summary>
    public void Click()
    {
        var element = CreateWait().Until<IWebElement?>(d =>
            d.FindElements(_locator).FirstOrDefault(e => e.Displayed && e.Enabled))!;
        SafeClick(element);
    }

    /// <summary>Нажатие без ожидания активности — для негативных проверок (неактивная кнопка).</summary>
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
            // Элемент перекрыт (липкая шапка, баннер) — кликаем через JavaScript
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", element);
        }
    }
}
