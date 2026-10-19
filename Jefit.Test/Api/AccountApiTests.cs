using Allure.Net.Commons;
using Allure.NUnit.Attributes;

namespace Jefit.Test.Api;

/// <summary>
/// Требуют тестовый аккаунт и пути API в appsettings (см. README). Без них тесты помечаются как Ignored.
/// </summary>
[TestFixture]
[Category("auth")]
[AllureFeature("Account")]
public class AccountApiTests : ApiTestBase
{
    private JefitAccountClient _account = null!;

    [SetUp]
    public void SetUpAccount()
    {
        if (!Settings.HasCredentials || !Settings.IsAccountApiConfigured)
        {
            Assert.Ignore("Не настроены тестовый аккаунт или пути Account API (appsettings.json).");
        }

        _account = new JefitAccountClient();
    }

    [TearDown]
    public void TearDownAccount() => _account?.Dispose();

    [Test]
    [AllureSeverity(SeverityLevel.critical)]
    public async Task Login_WithValidCredentials_ShouldSucceed()
    {
        var result = await _account.LoginAsync(new UserCredentials(Settings.UserEmail, Settings.UserPassword));

        Assert.That(result.IsSuccess, Is.True, $"Логин не удался: HTTP {result.Response.StatusCode}");
    }

    [Test]
    [AllureSeverity(SeverityLevel.normal)]
    public async Task Login_WithRandomCredentials_ShouldBeRejected()
    {
        var result = await _account.LoginAsync(TestDataGenerator.GenerateInvalidUser());

        Assert.That(result.IsSuccess, Is.False, "Случайные данные не должны открывать сессию");
    }
}
