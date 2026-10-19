using TechTalk.SpecFlow;

namespace Jefit.Test.StepDefinitions;

[Binding]
public class LoginSteps
{
    private readonly BrowserContext _browser;
    private LoginPage? _loginPage;

    public LoginSteps(BrowserContext browser) => _browser = browser;

    private LoginPage LoginPage => _loginPage ?? throw new InvalidOperationException("Страница входа не открыта.");

    [Given(@"I open the login page")]
    public void GivenIOpenTheLoginPage() => _loginPage = LoginPage.Open(_browser.Driver);

    [When(@"I log in with randomly generated invalid credentials")]
    public void WhenILogInWithRandomlyGeneratedInvalidCredentials()
    {
        var user = TestDataGenerator.GenerateInvalidUser();
        AllureHelper.AttachText("Сгенерированный email", user.Email);
        LoginPage.Login(user.Email, user.Password);
    }

    [When(@"I submit the login form with empty fields")]
    public void WhenISubmitTheLoginFormWithEmptyFields() => LoginPage.SubmitEmptyForm();

    [When(@"I log in with valid credentials")]
    public void WhenILogInWithValidCredentials() =>
        LoginPage.Login(ConfigReader.Settings.UserEmail, ConfigReader.Settings.UserPassword);

    [Then(@"the login form should be displayed")]
    public void ThenTheLoginFormShouldBeDisplayed() =>
        Assert.That(LoginPage.IsFormDisplayed(), Is.True, "Не отображаются поля email, пароль или кнопка входа");

    [Then(@"I should remain on the login page")]
    public void ThenIShouldRemainOnTheLoginPage() =>
        Assert.That(LoginPage.StaysOnLoginPage(), Is.True, "Пользователь не должен был войти в систему");

    [Then(@"I should be redirected away from the login page")]
    public void ThenIShouldBeRedirectedAwayFromTheLoginPage() =>
        Assert.That(LoginPage.WaitForLeavingLoginPage(), Is.True, "После входа не открылся личный кабинет");
}
