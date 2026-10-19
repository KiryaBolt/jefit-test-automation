using TechTalk.SpecFlow;

namespace Jefit.Test.StepDefinitions;

[Binding]
public class ExerciseDatabaseSteps
{
    private readonly BrowserContext _browser;
    private readonly ScenarioData _data;
    private ExercisesPage? _exercisesPage;
    private ExerciseDetailsPage? _detailsPage;

    public ExerciseDatabaseSteps(BrowserContext browser, ScenarioData data)
    {
        _browser = browser;
        _data = data;
    }

    private ExercisesPage ExercisesPage => _exercisesPage ?? throw new InvalidOperationException("База упражнений не открыта.");
    private ExerciseDetailsPage DetailsPage => _detailsPage ?? throw new InvalidOperationException("Упражнение не открыто.");

    [Given(@"I open the exercise database page")]
    public void GivenIOpenTheExerciseDatabasePage() => _exercisesPage = ExercisesPage.Open(_browser.Driver);

    [Given(@"I open the exercise details page ""(.*)""")]
    public void GivenIOpenTheExerciseDetailsPage(string path) =>
        _detailsPage = ExerciseDetailsPage.Open(_browser.Driver, path);

    [When(@"I filter exercises by muscle ""(.*)""")]
    public void WhenIFilterExercisesByMuscle(string muscle)
    {
        _data.InitialCount = ExercisesPage.GetFoundExercisesCount();
        ExercisesPage.FilterByMuscle(muscle);
    }

    [When(@"I go to the next page of exercises")]
    public void WhenIGoToTheNextPageOfExercises()
    {
        _data.FirstPageIds = ExercisesPage.GetExerciseCards().Select(c => c.Info.Id).ToList();
        ExercisesPage.GoToNextPage();
    }

    [When(@"I open the exercise ""(.*)"" from the list")]
    public void WhenIOpenTheExerciseFromTheList(string name) => _detailsPage = ExercisesPage.OpenExercise(name);

    [Then(@"the exercise counter should show more than (\d+) exercises")]
    public void ThenTheExerciseCounterShouldShowMoreThan(int count) =>
        Assert.That(ExercisesPage.GetFoundExercisesCount(), Is.GreaterThan(count), "Слишком мало упражнений в базе");

    [Then(@"at least (\d+) exercise cards should be displayed")]
    public void ThenAtLeastExerciseCardsShouldBeDisplayed(int count) =>
        Assert.That(ExercisesPage.GetExerciseCards(), Has.Count.GreaterThanOrEqualTo(count), "Отображается мало карточек");

    [Then(@"the number of found exercises should decrease")]
    public void ThenTheNumberOfFoundExercisesShouldDecrease() =>
        Assert.That(ExercisesPage.GetFoundExercisesCount(), Is.LessThan(_data.InitialCount),
            "После фильтрации количество упражнений не уменьшилось");

    [Then(@"every exercise card should belong to muscle group ""(.*)""")]
    public void ThenEveryExerciseCardShouldBelongToMuscleGroup(string muscle)
    {
        var wrongCards = ExercisesPage.GetExerciseCards()
            .Where(card => !card.Text.Contains(muscle, StringComparison.OrdinalIgnoreCase))
            .Select(card => card.Info.Name)
            .ToList();

        Assert.That(wrongCards, Is.Empty, $"Карточки не из группы {muscle}: {string.Join(", ", wrongCards)}");
    }

    [Then(@"the exercise list should differ from the first page")]
    public void ThenTheExerciseListShouldDifferFromTheFirstPage()
    {
        var currentIds = ExercisesPage.GetExerciseCards().Select(c => c.Info.Id);
        Assert.That(currentIds.Intersect(_data.FirstPageIds), Is.Empty, "На второй странице те же упражнения, что на первой");
    }

    [Then(@"the exercise page should contain the following sections:")]
    public void ThenTheExercisePageShouldContainTheFollowingSections(Table table)
    {
        var missing = table.Rows
            .Select(row => row["section"])
            .Where(section => !DetailsPage.HasSection(section))
            .ToList();

        Assert.That(missing, Is.Empty, $"На странице упражнения нет блоков: {string.Join(", ", missing)}");
    }
}
