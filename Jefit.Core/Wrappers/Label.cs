using System.Text.RegularExpressions;
using OpenQA.Selenium;

namespace Jefit.Core.Wrappers;

public class Label : BaseElement
{
    public Label(IWebDriver driver, By locator) : base(driver, locator)
    {
    }

    public int GetNumber()
    {
        var match = Regex.Match(Text, @"\d[\d,]*");
        if (!match.Success)
        {
            throw new InvalidOperationException($"В тексте \"{Text}\" нет числа.");
        }

        return int.Parse(match.Value.Replace(",", string.Empty));
    }
}
