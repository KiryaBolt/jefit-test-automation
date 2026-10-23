using Microsoft.Extensions.Configuration;

namespace Jefit.Core.Configuration;

public static class ConfigReader
{
    private static readonly Lazy<TestSettings> LazySettings =
        new(Load, LazyThreadSafetyMode.ExecutionAndPublication);

    public static TestSettings Settings => LazySettings.Value;

    private static TestSettings Load()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.local.json", optional: true)
            .AddEnvironmentVariables(prefix: "JEFIT_")
            .Build();

        var settings = new TestSettings();
        config.GetSection("TestSettings").Bind(settings);
        return settings;
    }
}
