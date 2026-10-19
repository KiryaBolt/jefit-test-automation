using System.Text.RegularExpressions;
using Jefit.Core.Api;
using Jefit.Core.Api.Models;
using Jefit.Core.Infrastructure;
using Jefit.Core.Wrappers;
using OpenQA.Selenium;

namespace Jefit.Core.PageObjects;

/// <summary>База планов тренировок https://www.jefit.com/routines (и категории /routines/beginner и т.д.)</summary>
public class RoutinesPage : BasePage
{
    private static readonly By CardLinks = By.CssSelector("a[href*='/routines/']");
    private static readonly By PlansCounterCandidates = By.XPath(XPathHelper.DeepestContaining("plans"));
    private static readonly Regex PlansCounterRegex = new(@"^(\d[\d,]*)\s*plans$", RegexOptions.IgnoreCase);

    public RoutinesPage(IWebDriver driver) : base(driver)
    {
    }

    private Label Heading => new(_driver, By.TagName("h1"));

    public static RoutinesPage Open(IWebDriver driver)
    {
        NavigateTo(driver, "/routines");
        return new RoutinesPage(driver);
    }

    /// <summary>Число из надписи "26850 plans". Слово "plans" встречается и в меню, поэтому ищем текст строго вида "N plans".</summary>
    public int GetPlansCount() =>
        _wait.Until(d =>
        {
            foreach (var element in d.FindElements(PlansCounterCandidates))
            {
                var match = PlansCounterRegex.Match(element.Text.Trim());
                if (element.Displayed && match.Success)
                {
                    return (int?)int.Parse(match.Groups[1].Value.Replace(",", string.Empty));
                }
            }

            return null;
        })!.Value;

    public IReadOnlyList<RoutineInfo> GetRoutineCards() =>
        _wait.Until(_ =>
        {
            var cards = ReadCards();
            return cards.Count > 0 ? cards : null;
        })!;

    public RoutinesPage SelectCategory(string category)
    {
        var slug = category.Trim().ToLowerInvariant().Replace(' ', '-');
        new Link(_driver, By.CssSelector($"a[href$='/routines/{slug}']")).Click();
        WaitForUrlContains($"/routines/{slug}");
        return new RoutinesPage(_driver);
    }

    public RoutineDetailsPage OpenRoutine(RoutineInfo routine)
    {
        new Link(_driver, By.CssSelector($"a[href*='/routines/{routine.Id}/']")).Click();
        WaitForUrlContains($"/routines/{routine.Id}/");
        return new RoutineDetailsPage(_driver);
    }

    protected override void WaitForPageLoad() => EnsureDisplayed(Heading, "Планы тренировок");

    private List<RoutineInfo> ReadCards()
    {
        var cards = new List<RoutineInfo>();
        foreach (var link in _driver.FindElements(CardLinks))
        {
            var match = CardParser.RoutineHref.Match(link.GetAttribute("href") ?? string.Empty);
            if (!match.Success || !link.Displayed) continue;

            var alt = link.FindElements(By.TagName("img"))
                .Select(img => img.GetAttribute("alt"))
                .FirstOrDefault(a => !string.IsNullOrWhiteSpace(a));
            var slug = match.Groups[2].Value;
            cards.Add(new RoutineInfo(int.Parse(match.Groups[1].Value), slug,
                CardParser.ResolveName(alt, slug, CardParser.RoutineAltSuffix), match.Value));
        }

        return cards.DistinctBy(c => c.Id).ToList();
    }
}
