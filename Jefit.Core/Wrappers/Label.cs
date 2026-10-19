using System.Text.RegularExpressions;
using OpenQA.Selenium;

namespace Jefit.Core.Wrappers;

/// <summary>Текстовый элемент: заголовок, надпись, счётчик.</summary>
public class Label : BaseElement
{
    public Label(IWebDriver driver, By locator) : base(driver, locator)
    {
    }

    /// <summary>Первое число в тексте: "1269 EXERCISES FOUND" -> 1269, "26,850 plans" -> 26850.</summary>
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
