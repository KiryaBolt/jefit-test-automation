using TechTalk.SpecFlow;
using TechTalk.SpecFlow.UnitTestProvider;

namespace Jefit.Test.Hooks;

/// <summary>
/// Сценарии @auth требуют тестовый аккаунт (а сценарий с API — ещё и пути Account API).
/// Если они не настроены, сценарий помечается Ignored, а не падает.
/// </summary>
[Binding]
public class AuthHooks
{
    private readonly ScenarioContext _scenarioContext;
    private readonly IUnitTestRuntimeProvider _testRuntime;
    private readonly ScenarioData _data;
    private readonly JefitAccountClient _account;

    public AuthHooks(ScenarioContext scenarioContext, IUnitTestRuntimeProvider testRuntime,
        ScenarioData data, JefitAccountClient account)
    {
        _scenarioContext = scenarioContext;
        _testRuntime = testRuntime;
        _data = data;
        _account = account;
    }

    [BeforeScenario("auth", Order = -100)]
    public void SkipIfNotConfigured()
    {
        var settings = ConfigReader.Settings;
        if (!settings.HasCredentials)
        {
            _testRuntime.TestIgnore("Не задан тестовый аккаунт: UserEmail / UserPassword в appsettings.");
            return;
        }

        var usesApi = _scenarioContext.ScenarioInfo.Tags.Contains("api-data") ||
                      _scenarioContext.ScenarioInfo.Title.Contains("API", StringComparison.OrdinalIgnoreCase);
        if (usesApi && !settings.IsAccountApiConfigured)
        {
            _testRuntime.TestIgnore("Не настроены пути Account API (AccountLoginPath и др.).");
        }
    }

    /// <summary>Удаляет созданные через API данные, даже если сценарий упал.</summary>
    [AfterScenario("auth")]
    public void DeleteCreatedData()
    {
        if (_data.CreatedPlanId is null) return;
        _account.DeleteRoutineAsync(_data.CreatedPlanId).GetAwaiter().GetResult();
        _data.CreatedPlanId = null;
    }
}
