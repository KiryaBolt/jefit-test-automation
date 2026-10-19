using Allure.Net.Commons;
using Allure.NUnit.Attributes;

namespace Jefit.Test.Api;

[TestFixture]
[AllureFeature("Exercise database")]
public class ExerciseDatabaseApiTests : ApiTestBase
{
    [Test]
    [Category("smoke")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureDescription("Сервер отдаёт список упражнений уже в HTML (серверный рендеринг)")]
    public async Task ExerciseList_ShouldContainExercises()
    {
        var response = await Client.GetPageAsync("/exercises");
        var exercises = HtmlPageParser.GetExercises(response.Body);
        var meta = HtmlPageParser.GetMeta(response.Body);

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(200));
            Assert.That(exercises, Has.Count.GreaterThanOrEqualTo(10), "На странице меньше 10 упражнений");
            Assert.That(exercises.Select(e => e.Name), Has.All.Not.Empty, "У упражнения нет названия");
            Assert.That(meta.Canonical?.TrimEnd('/'), Is.EqualTo("https://www.jefit.com/exercises"), "Неверный canonical URL");
        });
    }

    [TestCase("/exercises/2/barbell-bench-press", "Barbell Bench Press")]
    [TestCase("/exercises/12/barbell-squat", "Barbell Squat")]
    [TestCase("/exercises/32/dumbbell-lateral-raise", "Dumbbell Lateral Raise")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription("SEO-теги страницы упражнения: title, og:title, H1, description")]
    public async Task ExercisePage_ShouldHaveCorrectSeoData(string path, string name)
    {
        var response = await Client.GetPageAsync(path);
        var meta = HtmlPageParser.GetMeta(response.Body);

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(200));
            Assert.That(meta.Title, Is.EqualTo($"{name} - JEFIT"), "Неверный title");
            Assert.That(meta.OgTitle, Is.EqualTo($"{name} - JEFIT"), "Неверный og:title");
            Assert.That(meta.H1, Is.EqualTo(name), "Неверный заголовок H1");
            Assert.That(meta.Description, Is.Not.Null.And.Not.Empty, "Нет meta description");
        });
    }

    [Test]
    [AllureSeverity(SeverityLevel.normal)]
    public async Task Pagination_ShouldReturnDifferentExercises()
    {
        var firstPage = await Client.GetExercisesAsync(page: 1);
        var secondPage = await Client.GetExercisesAsync(page: 2);

        Assert.Multiple(() =>
        {
            Assert.That(secondPage, Is.Not.Empty, "Вторая страница пустая");
            Assert.That(secondPage.Select(e => e.Id).Intersect(firstPage.Select(e => e.Id)), Is.Empty,
                "Страницы 1 и 2 содержат одинаковые упражнения");
        });
    }

    [TestCase("chest")]
    [TestCase("back")]
    [TestCase("biceps")]
    [TestCase("upper_legs")]
    [AllureSeverity(SeverityLevel.normal)]
    public async Task MuscleGroupPage_ShouldContainExercises(string muscle)
    {
        var response = await Client.GetPageAsync($"/exercises/{muscle}");

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(200));
            Assert.That(HtmlPageParser.GetExercises(response.Body), Has.Count.GreaterThanOrEqualTo(5),
                $"На странице группы мышц {muscle} мало упражнений");
        });
    }
}
