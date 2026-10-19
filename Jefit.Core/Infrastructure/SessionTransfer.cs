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
        driver.Navigate().GoToUrl(baseUrl);
        foreach (System.Net.Cookie cookie in cookies.GetAllCookies())
        {
            DateTime? expiry = cookie.Expires == DateTime.MinValue ? null : cookie.Expires;
            driver.Manage().Cookies.AddCookie(
                new OpenQA.Selenium.Cookie(cookie.Name, cookie.Value, cookie.Domain, cookie.Path, expiry));
        }

        driver.Navigate().Refresh();
    }
}
