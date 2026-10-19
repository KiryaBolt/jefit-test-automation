using TechTalk.SpecFlow;

namespace Jefit.Test.StepDefinitions;

[Binding]
public class WorkoutPlansSteps
{
    private readonly BrowserContext _browser;
    private readonly ScenarioData _data;
    private RoutinesPage? _routinesPage;

    public WorkoutPlansSteps(BrowserContext browser, ScenarioData data)
    {
        _browser = browser;
        _data = data;
    }

    private RoutinesPage RoutinesPage => _routinesPage ?? throw new InvalidOperationException("Страница планов не открыта.");

    [Given(@"I open the workout plans page")]
    public void GivenIOpenTheWorkoutPlansPage() => _routinesPage = RoutinesPage.Open(_browser.Driver);

    [When(@"I select the workout plan category ""(.*)""")]
    public void WhenISelectTheWorkoutPlanCategory(string category) =>
        _routinesPage = RoutinesPage.SelectCategory(category);

    [When(@"I open the first workout plan from the list")]
    public void WhenIOpenTheFirstWorkoutPlanFromTheList()
    {
        var firstPlan = RoutinesPage.GetRoutineCards()[0];
        _data.RememberedName = firstPlan.Name;
        RoutinesPage.OpenRoutine(firstPlan);
    }

    [Then(@"the plans counter should show more than (\d+) plans")]
    public void ThenThePlansCounterShouldShowMoreThan(int count) =>
        Assert.That(RoutinesPage.GetPlansCount(), Is.GreaterThan(count), "Слишком мало планов в базе");

    [Then(@"at least (\d+) workout plan cards should be displayed")]
    public void ThenAtLeastWorkoutPlanCardsShouldBeDisplayed(int count) =>
        Assert.That(RoutinesPage.GetRoutineCards(), Has.Count.GreaterThanOrEqualTo(count), "Отображается мало планов");
}
