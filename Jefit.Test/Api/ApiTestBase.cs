using Allure.NUnit;
using Allure.NUnit.Attributes;

namespace Jefit.Test.Api;

[AllureNUnit]
[AllureEpic("API")]
[Category("api")]
[Category("regression")]
public abstract class ApiTestBase
{
    protected JefitWebClient Client = null!;
    protected TestSettings Settings = null!;

    [SetUp]
    public void SetUp()
    {
        Settings = ConfigReader.Settings;
        Client = new JefitWebClient();
    }

    [TearDown]
    public void TearDown()
    {
        Client.Dispose();
    }
}
