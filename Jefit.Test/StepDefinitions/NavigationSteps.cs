using TechTalk.SpecFlow;

namespace Jefit.Test.StepDefinitions;

[Binding]
public class NavigationSteps
{
    private readonly BrowserContext _browser;
    private HomePage? _homePage;

    public NavigationSteps(BrowserContext browser) => _browser = browser;

    private HomePage HomePage => _homePage ?? throw new InvalidOperationException("Главная страница не открыта.");

    [Given(@"I am on the Jefit home page")]
    public void GivenIAmOnTheJefitHomePage() => _homePage = HomePage.Open(_browser.Driver);

    [When(@"I click the Get Started button")]
    public void WhenIClickTheGetStartedButton() => HomePage.ClickGetStarted();

    [When(@"I click the ""(.*)"" link in the footer")]
    public void WhenIClickTheLinkInTheFooter(string link) => HomePage.Footer.ClickLink(link);

    [Then(@"the footer link ""(.*)"" should point to ""(.*)""")]
    public void ThenTheFooterLinkShouldPointTo(string link, string address) =>
        Assert.That(HomePage.Footer.GetLinkHref(link), Does.Contain(address).IgnoreCase,
            $"Ссылка \"{link}\" ведёт не туда");
}
