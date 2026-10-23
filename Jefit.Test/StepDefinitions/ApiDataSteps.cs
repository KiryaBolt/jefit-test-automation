using TechTalk.SpecFlow;

namespace Jefit.Test.StepDefinitions;

[Binding]
public class ApiDataSteps : IDisposable
{
    private readonly ScenarioData _data;
    private readonly JefitWebClient _client = new();

    public ApiDataSteps(ScenarioData data) => _data = data;

    [Given(@"I pick a random exercise from the exercise database via API")]
    public async Task GivenIPickARandomExerciseFromTheExerciseDatabaseViaApi()
    {
        var exercise = TestDataGenerator.PickRandom(await _client.GetExercisesAsync());
        Remember(exercise.Name, exercise.RelativeUrl);
    }

    [Given(@"I pick a random workout plan from the routine database via API")]
    public async Task GivenIPickARandomWorkoutPlanFromTheRoutineDatabaseViaApi()
    {
        var routine = TestDataGenerator.PickRandom(await _client.GetRoutinesAsync());
        Remember(routine.Name, routine.RelativeUrl);
    }

    public void Dispose() => _client.Dispose();

    private void Remember(string name, string path)
    {
        _data.RememberedName = name;
        _data.RememberedPath = path;
        AllureHelper.AttachText("Тестовые данные из API", $"{name}\n{path}");
    }
}
