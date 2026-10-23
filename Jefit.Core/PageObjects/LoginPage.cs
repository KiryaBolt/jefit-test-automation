using Jefit.Core.Infrastructure;
using Jefit.Core.Wrappers;
using OpenQA.Selenium;

namespace Jefit.Core.PageObjects;

public class LoginPage : BasePage
{
    public LoginPage(IWebDriver driver) : base(driver)
    {
    }

    private Input EmailInput => new(_driver, By.CssSelector(
        "input[type='email'], input[name='email'], input[name='username'], " +
        "input[autocomplete='username'], input[autocomplete='email']"));

    private Input PasswordInput => new(_driver, By.CssSelector("input[type='password']"));

    private Button SubmitButton => new(_driver, By.XPath(
        "//form//button[@type='submit'] | " +
        $"//button[contains({XPathHelper.LowerText()}, 'log in') or contains({XPathHelper.LowerText()}, 'login') " +
        $"or contains({XPathHelper.LowerText()}, 'sign in')]"));

    public static LoginPage Open(IWebDriver driver)
    {
        NavigateTo(driver, "/login");
        return new LoginPage(driver);
    }

    public bool IsFormDisplayed() =>
        EmailInput.IsDisplayed() && PasswordInput.IsDisplayed() && SubmitButton.IsDisplayed();

    public void Login(string email, string password)
    {
        EmailInput.SendKeys(email);
        PasswordInput.SendKeys(password);
        SubmitButton.Click();
    }

    public void SubmitEmptyForm() => SubmitButton.ClickWithoutWaitingEnabled();

    public bool StaysOnLoginPage(int seconds = 5) =>
        WaitHelper.ConditionHoldsFor(
            () => _driver.Url.Contains("/login", StringComparison.OrdinalIgnoreCase),
            TimeSpan.FromSeconds(seconds));

    public bool WaitForLeavingLoginPage() =>
        WaitUntil(d => !d.Url.Contains("/login", StringComparison.OrdinalIgnoreCase));

    protected override void WaitForPageLoad() => EnsureDisplayedWithReload(EmailInput, "Вход");
}
