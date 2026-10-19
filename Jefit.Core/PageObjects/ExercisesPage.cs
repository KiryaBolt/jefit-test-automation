using Jefit.Core.Api;
using Jefit.Core.Api.Models;
using Jefit.Core.Infrastructure;
using Jefit.Core.Wrappers;
using OpenQA.Selenium;

namespace Jefit.Core.PageObjects;

/// <summary>База упражнений https://www.jefit.com/exercises</summary>
public class ExercisesPage : BasePage
{
    private static readonly By CardLinks = By.CssSelector("a[href*='/exercises/']");

    public ExercisesPage(IWebDriver driver) : base(driver)
    {
    }

    private Label Heading => new(_driver, By.XPath("//h1[normalize-space()='Exercise Database']"));
    private Label FoundCounter => new(_driver, By.XPath(XPathHelper.DeepestContaining("exercises found")));
    private Link NextPageLink => new(_driver, By.XPath("//a[normalize-space()='Next']"));

    private static By MuscleFilterLocator(string muscle) => By.XPath(
        $"(//h2[normalize-space()='Select by Muscle']/following::*[normalize-space()={XPathHelper.Literal(muscle)}])[1]");

    public static ExercisesPage Open(IWebDriver driver)
    {
        NavigateTo(driver, "/exercises");
        return new ExercisesPage(driver);
    }

    /// <summary>Число из надписи "1269 EXERCISES FOUND".</summary>
    public int GetFoundExercisesCount() => FoundCounter.GetNumber();

    public IReadOnlyList<ExerciseCard> GetExerciseCards() =>
        _wait.Until(_ =>
        {
            var cards = ReadCards();
            return cards.Count > 0 ? cards : null;
        })!;

    public void FilterByMuscle(string muscle)
    {
        var locator = MuscleFilterLocator(muscle);
        var countBefore = GetFoundExercisesCount();

        // Страница показывается раньше, чем JavaScript сайта подключается к кнопкам.
        // Клик до этого момента ничего не делает, поэтому ждём "оживления" кнопки.
        WaitUntilInteractive(locator);

        new Button(_driver, locator).Click();
        WaitUntil(_ => GetFoundExercisesCount() != countBefore);
    }

    public void GoToNextPage()
    {
        var firstIdBefore = GetExerciseCards()[0].Info.Id;
        NextPageLink.Click();
        WaitUntil(_ => ReadCards().FirstOrDefault()?.Info.Id is int id && id != firstIdBefore);
    }

    public ExerciseDetailsPage OpenExercise(string name)
    {
        var card = GetExerciseCards().FirstOrDefault(c => TextHelper.Normalize(c.Info.Name) == TextHelper.Normalize(name))
                   ?? throw new InvalidOperationException($"Упражнение \"{name}\" не найдено на текущей странице.");

        new Link(_driver, By.CssSelector($"a[href*='/exercises/{card.Info.Id}/']")).Click();
        WaitForUrlContains($"/exercises/{card.Info.Id}/");
        return new ExerciseDetailsPage(_driver);
    }

    protected override void WaitForPageLoad() => EnsureDisplayed(Heading, "База упражнений");

    /// <summary>React добавляет элементу служебные свойства __react..., когда подключает к нему обработчики.</summary>
    private void WaitUntilInteractive(By locator) =>
        WaitUntil(d =>
        {
            var element = d.FindElements(locator).FirstOrDefault();
            if (element is null) return false;

            var result = ((IJavaScriptExecutor)d).ExecuteScript(
                "return Object.keys(arguments[0]).some(k => k.startsWith('__react'));", element);
            return Equals(result, true);
        });

    private List<ExerciseCard> ReadCards()
    {
        var cards = new List<ExerciseCard>();
        foreach (var link in _driver.FindElements(CardLinks))
        {
            var match = CardParser.ExerciseHref.Match(link.GetAttribute("href") ?? string.Empty);
            if (!match.Success || !link.Displayed) continue;

            var alt = link.FindElements(By.TagName("img"))
                .Select(img => img.GetAttribute("alt"))
                .FirstOrDefault(a => !string.IsNullOrWhiteSpace(a));
            var slug = match.Groups[2].Value;
            var info = new ExerciseInfo(int.Parse(match.Groups[1].Value), slug,
                CardParser.ResolveName(alt, slug, CardParser.ExerciseAltSuffix), match.Value);

            cards.Add(new ExerciseCard(info, link.Text));
        }

        return cards.DistinctBy(c => c.Info.Id).ToList();
    }
}
