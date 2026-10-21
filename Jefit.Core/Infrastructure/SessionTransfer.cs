using System.Net;
using OpenQA.Selenium;

namespace Jefit.Core.Infrastructure;

/// <summary>
/// Переносит cookie сессии из API-клиента (RestSharp) в браузер:
/// UI-тест сразу начинается авторизованным, без прохождения формы логина.
/// </summary>
public static class SessionTransfer
{
    public static void CopyCookiesToBrowser(IWebDriver driver, CookieContainer cookies, string baseUrl)
    {
        // Cookie можно добавить только на странице нужного домена. Открываем не главную (она тяжёлая
        // и может грузиться дольше таймаута), а лёгкий текстовый файл того же сайта.
        try
        {
            driver.Navigate().GoToUrl(baseUrl.TrimEnd('/') + "/robots.txt");
        }
        catch (WebDriverTimeoutException)
        {
            try
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("window.stop();");
            }
            catch (WebDriverException)
            {
                // браузер уже ответил — останавливать нечего
            }
        }

        foreach (System.Net.Cookie cookie in cookies.GetAllCookies())
        {
            DateTime? expiry = cookie.Expires == DateTime.MinValue ? null : cookie.Expires;
            // Без домена: cookie привязывается к текущему сайту (www.jefit.com)
            driver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie(cookie.Name, cookie.Value, "/", expiry));
        }
    }
}
