using TechTalk.SpecFlow;

namespace Jefit.Test.StepDefinitions;

[Binding]
public class CommonSteps
{
    private readonly BrowserContext _browser;
    private readonly ScenarioData _data;

    public CommonSteps(BrowserContext browser, ScenarioData data)
    {
        _browser = browser;
        _data = data;
    }

    private GenericPage CurrentPage => new(_browser.Driver);

    [When(@"I open the selected item in the browser")]
    public void WhenIOpenTheSelectedItemInTheBrowser() =>
        GenericPage.Open(_browser.Driver, _data.RememberedPath ?? throw new InvalidOperationException("Элемент не выбран."));

    [Then(@"the current URL should contain ""(.*)""")]
    public void ThenTheCurrentUrlShouldContain(string part)
    {
        var page = CurrentPage;
        page.WaitForUrlContains(part);
        Assert.That(page.GetUrl(), Does.Contain(part).IgnoreCase, "Открылась не та страница");
    }

    [Then(@"the page title should contain ""(.*)""")]
    public void ThenThePageTitleShouldContain(string text)
    {
        var page = CurrentPage;
        page.WaitForTitleContains(text);
        Assert.That(page.GetPageTitle(), Does.Contain(text).IgnoreCase, "Неверный title страницы");
    }

    [Then(@"the main heading should be ""(.*)""")]
    public void ThenTheMainHeadingShouldBe(string heading) =>
        Assert.That(CurrentPage.GetMainHeading(), Is.EqualTo(heading), "Неверный заголовок страницы");

    [Then(@"the page heading should match the remembered name")]
    public void ThenThePageHeadingShouldMatchTheRememberedName()
    {
        var expected = RememberedName();
        var actual = CurrentPage.GetMainHeading();
        Assert.That(TextHelper.Normalize(actual), Is.EqualTo(TextHelper.Normalize(expected)),
            $"Заголовок \"{actual}\" не совпадает с ожидаемым \"{expected}\"");
    }

    [Then(@"the page title should contain the remembered name")]
    public void ThenThePageTitleShouldContainTheRememberedName()
    {
        var expected = RememberedName();
        var title = CurrentPage.GetPageTitle();
        Assert.That(TextHelper.Normalize(title), Does.Contain(TextHelper.Normalize(expected)),
            $"Title \"{title}\" не содержит \"{expected}\"");
    }

    private string RememberedName() =>
        _data.RememberedName ?? throw new InvalidOperationException("В предыдущих шагах не сохранено название.");
}
