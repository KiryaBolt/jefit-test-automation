using System.Runtime.InteropServices;
using TechTalk.SpecFlow;

namespace Jefit.Test.Hooks;

[Binding]
public static class TestRunHooks
{
    [BeforeTestRun]
    public static void PrepareAllureResults()
    {
        var settings = ConfigReader.Settings;
        AllureHelper.WriteEnvironment(new Dictionary<string, string>
        {
            ["Base.URL"] = settings.BaseUrl,
            ["Browser"] = settings.BrowserType,
            ["Headless"] = settings.IsHeadless.ToString(),
            ["OS"] = RuntimeInformation.OSDescription,
            ["Runtime"] = RuntimeInformation.FrameworkDescription
        });
        AllureHelper.CopyToResults(Path.Combine(AppContext.BaseDirectory, "categories.json"));
    }
}
