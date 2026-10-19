using System.Runtime.InteropServices;
using OpenQA.Selenium;

namespace Jefit.Core.Wrappers;

public class Input : BaseElement
{
    public Input(IWebDriver driver, By locator) : base(driver, locator)
    {
    }

    public string Value => GetAttribute("value");

    public void SendKeys(string text)
    {
        var element = Element;
        Clear(element);
        element.SendKeys(text);
    }

    // Clear() на React-полях иногда не срабатывает — тогда очищаем через выделение всего текста.
    // Cmd+A на macOS и Ctrl+A на Windows/Linux, чтобы не напечатать лишнюю "a".
    private static void Clear(IWebElement element)
    {
        element.Clear();
        if (string.IsNullOrEmpty(element.GetAttribute("value"))) return;

        var modifier = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? Keys.Command : Keys.Control;
        element.SendKeys(modifier + "a");
        element.SendKeys(Keys.Delete);
    }
}
