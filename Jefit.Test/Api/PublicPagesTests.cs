using Allure.Net.Commons;
using Allure.NUnit.Attributes;

namespace Jefit.Test.Api;

[TestFixture]
[AllureFeature("Public pages")]
public class PublicPagesTests : ApiTestBase
{
    [TestCase("/", "Workout Planner")]
    [TestCase("/exercises", "Exercise Database")]
    [TestCase("/routines", "Workout Routine Database")]
    [TestCase("/login", "Login")]
    [TestCase("/signup", "JEFIT")]
    [TestCase("/elite", "JEFIT")]
    [TestCase("/blog", "JEFIT")]
    [TestCase("/support/faq", "JEFIT")]
    [Category("smoke")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureDescription("Страница отвечает 200, отдаёт HTML с нужным title и укладывается в лимит времени")]
    public async Task PublicPage_ShouldBeAvailable(string path, string expectedTitle)
    {
        var response = await Client.GetPageAsync(path);
        var meta = HtmlPageParser.GetMeta(response.Body);

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(200), $"Страница {path} вернула неверный статус");
            Assert.That(response.ContentType, Does.Contain("text/html").IgnoreCase, "Ожидался HTML");
            Assert.That(meta.Title, Does.Contain(expectedTitle).IgnoreCase, "Неверный title страницы");
            Assert.That(response.Elapsed.TotalMilliseconds, Is.LessThan(Settings.MaxResponseTimeMs),
                $"Страница {path} отвечает дольше {Settings.MaxResponseTimeMs} мс");
        });
    }

    [Test]
    [AllureSeverity(SeverityLevel.normal)]
    public async Task NotExistingPage_ShouldReturn404()
    {
        var response = await Client.GetPageAsync("/this-page-does-not-exist-qa-check");

        Assert.That(response.StatusCode, Is.EqualTo(404), "Для несуществующей страницы ожидался 404");
    }

    [Test]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureDescription("Запрос по http:// перенаправляется на https://")]
    public async Task HttpRequest_ShouldRedirectToHttps()
    {
        var response = await Client.GetPageAsync("http://www.jefit.com/", followRedirects: false);

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.AnyOf(301, 302, 307, 308), "Ожидался редирект");
            Assert.That(response.GetHeader("Location"), Does.StartWith("https://").IgnoreCase,
                "Редирект должен вести на https");
        });
    }

    [Test]
    [AllureSeverity(SeverityLevel.minor)]
    public async Task RobotsTxt_ShouldBeAvailable()
    {
        var response = await Client.GetPageAsync("/robots.txt");

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(200));
            Assert.That(response.Body, Does.Contain("User-agent").IgnoreCase, "В robots.txt нет правил User-agent");
        });
    }
}
