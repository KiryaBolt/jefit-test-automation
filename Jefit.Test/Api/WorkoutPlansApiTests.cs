using Allure.Net.Commons;
using Allure.NUnit.Attributes;

namespace Jefit.Test.Api;

[TestFixture]
[AllureFeature("Workout plans")]
public class WorkoutPlansApiTests : ApiTestBase
{
    [Test]
    [Category("smoke")]
    [AllureSeverity(SeverityLevel.critical)]
    public async Task RoutineList_ShouldContainPlans()
    {
        var response = await Client.GetPageAsync("/routines");

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(200));
            Assert.That(HtmlPageParser.GetRoutines(response.Body), Has.Count.GreaterThanOrEqualTo(10),
                "На странице меньше 10 планов");
        });
    }

    [TestCase("beginner")]
    [TestCase("home")]
    [TestCase("gym")]
    [TestCase("women")]
    [TestCase("muscle-building")]
    [AllureSeverity(SeverityLevel.normal)]
    public async Task RoutineCategory_ShouldContainPlans(string category)
    {
        var response = await Client.GetPageAsync($"/routines/{category}");

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(200));
            Assert.That(HtmlPageParser.GetRoutines(response.Body), Is.Not.Empty, $"В категории {category} нет планов");
        });
    }
}
