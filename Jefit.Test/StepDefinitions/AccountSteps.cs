using TechTalk.SpecFlow;

namespace Jefit.Test.StepDefinitions;

/// <summary>Вход через API, добавление плана через API, проверка в UI. План удаляется в AuthHooks после сценария.</summary>
[Binding]
public class AccountSteps
{
    private readonly JefitAccountClient _account;
    private readonly BrowserContext _browser;
    private readonly ScenarioData _data;

    public AccountSteps(JefitAccountClient account, BrowserContext browser, ScenarioData data)
    {
        _account = account;
        _browser = browser;
        _data = data;
    }

    [Given(@"I am logged in via API")]
    public async Task GivenIAmLoggedInViaApi()
    {
        var settings = ConfigReader.Settings;
        var result = await _account.LoginAsync(new UserCredentials(settings.UserEmail, settings.UserPassword));
        Assert.That(result.IsSuccess, Is.True, $"Вход через API не удался: HTTP {result.Response.StatusCode}");
    }

    [Given(@"I have added the catalog workout plan (\d+) ""(.*)"" via API")]
    public async Task GivenIHaveAddedTheCatalogWorkoutPlanViaApi(int catalogRoutineId, string name)
    {
        _data.CreatedPlanId = await _account.AddRoutineFromCatalogAsync(catalogRoutineId);
        _data.RememberedName = name;
        AllureHelper.AttachText("Добавлено через API", $"{name} (план пользователя {_data.CreatedPlanId})");
    }

    [When(@"I transfer the API session to the browser")]
    public void WhenITransferTheApiSessionToTheBrowser() =>
        SessionTransfer.CopyCookiesToBrowser(_browser.Driver, _account.Cookies, ConfigReader.Settings.BaseUrl);

    [When(@"I open the my plans page")]
    public void WhenIOpenTheMyPlansPage() =>
        GenericPage.Open(_browser.Driver, ConfigReader.Settings.MyPlansPath);

    [Then(@"the added workout plan should be displayed")]
    public void ThenTheAddedWorkoutPlanShouldBeDisplayed()
    {
        var page = new GenericPage(_browser.Driver);
        var name = _data.RememberedName!;
        var isDisplayed = page.WaitUntil(_ => page.GetBodyText().Contains(name, StringComparison.OrdinalIgnoreCase));
        Assert.That(isDisplayed, Is.True, $"План \"{name}\", добавленный через API, не отображается в «Моих планах»");
    }
}
