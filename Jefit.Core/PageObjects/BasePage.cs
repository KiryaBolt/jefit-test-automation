using Jefit.Core.Configuration;
using Jefit.Core.Wrappers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Jefit.Core.PageObjects;

/// <summary>
/// Базовая страница. Объект страницы создаётся, когда страница уже открыта:
/// конструктор дожидается загрузки документа, закрывает cookie-баннер и вызывает WaitForPageLoad().
///
/// Важно: WaitForPageLoad() вызывается из конструктора базового класса — до того,
/// как выполнится конструктор наследника. Поэтому элементы в наследниках объявлены
/// как свойства (=&gt; new Button(...)), а не как поля, заполняемые в конструкторе.
/// </summary>
public abstract class BasePage
{
    protected readonly IWebDriver _driver;
    protected readonly WebDriverWait _wait;

    protected BasePage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(ConfigReader.Settings.ExplicitWaitTimeout));
        _wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));

        WaitForDocumentReady();
        CookieBanner.AcceptIfPresent(driver);
        WaitForPageLoad();
    }

    /// <summary>Признак того, что именно эта страница загрузилась (у каждой страницы свой).</summary>
    protected abstract void WaitForPageLoad();

    public string GetUrl() => _driver.Url;

    public string GetPageTitle() => _driver.Title;

    public string GetMainHeading() => new Label(_driver, By.TagName("h1")).Text;

    /// <summary>Ждёт условие; при таймауте возвращает false, а не бросает исключение.</summary>
    public bool WaitUntil(Func<IWebDriver, bool> condition)
    {
        try
        {
            _wait.Until(condition);
            return true;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public bool WaitForUrlContains(string part) =>
        WaitUntil(d => d.Url.Contains(part, StringComparison.OrdinalIgnoreCase));

    public bool WaitForTitleContains(string part) =>
        WaitUntil(d => d.Title.Contains(part, StringComparison.OrdinalIgnoreCase));

    /// <summary>Переход по относительному пути или полному адресу.</summary>
    protected static void NavigateTo(IWebDriver driver, string pathOrUrl)
    {
        var url = pathOrUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? pathOrUrl
            : $"{ConfigReader.Settings.BaseUrl.TrimEnd('/')}/{pathOrUrl.TrimStart('/')}";
        driver.Navigate().GoToUrl(url);
    }

    /// <summary>Бросает понятную ошибку, если ключевой элемент страницы не появился.</summary>
    protected void EnsureDisplayed(BaseElement element, string pageName)
    {
        if (!element.IsDisplayed(ConfigReader.Settings.ExplicitWaitTimeout))
        {
            throw new InvalidOperationException($"Страница \"{pageName}\" не загрузилась. Текущий адрес: {_driver.Url}");
        }
    }

    private void WaitForDocumentReady() =>
        _wait.Until(d =>
        {
            var state = ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState")?.ToString();
            return state is "interactive" or "complete";
        });
}
