using System.Net;
using OpenQA.Selenium;

namespace Jefit.Core.Infrastructure;

public static class SessionTransfer
{
    public static void CopyCookiesToBrowser(IWebDriver driver, CookieContainer cookies, string baseUrl)
    {
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
            }
        }

        foreach (System.Net.Cookie cookie in cookies.GetAllCookies())
        {
            DateTime? expiry = cookie.Expires == DateTime.MinValue ? null : cookie.Expires;
            driver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie(cookie.Name, cookie.Value, "/", expiry));
        }
    }
}
