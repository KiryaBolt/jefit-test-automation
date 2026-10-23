using System.Diagnostics;
using System.Net;
using Jefit.Core.Configuration;
using Jefit.Core.Infrastructure;
using RestSharp;

namespace Jefit.Core.Api;

public abstract class BaseApiClient : IDisposable
{
    private readonly RestClient _client;
    private readonly RestClient _noRedirectClient;

    protected BaseApiClient(string baseUrl, CookieContainer? cookies = null)
    {
        _client = new RestClient(CreateOptions(baseUrl, followRedirects: true, cookies));
        _noRedirectClient = new RestClient(CreateOptions(baseUrl, followRedirects: false, cookies));
    }

    protected static TestSettings Settings => ConfigReader.Settings;

    protected async Task<ApiResponse> SendAsync(RestRequest request, bool followRedirects = true)
    {
        var client = followRedirects ? _client : _noRedirectClient;

        var stopwatch = Stopwatch.StartNew();
        var raw = await client.ExecuteAsync(request);
        stopwatch.Stop();

        var response = new ApiResponse(raw, stopwatch.Elapsed);
        AllureHelper.AttachApiCall(request.Method.ToString().ToUpperInvariant(),
            client.BuildUri(request).ToString(), response);
        return response;
    }

    public void Dispose()
    {
        _client.Dispose();
        _noRedirectClient.Dispose();
        GC.SuppressFinalize(this);
    }

    private static RestClientOptions CreateOptions(string baseUrl, bool followRedirects, CookieContainer? cookies) =>
        new(baseUrl)
        {
            Timeout = TimeSpan.FromSeconds(Settings.ApiTimeout),
            FollowRedirects = followRedirects,
            UserAgent = Settings.UserAgent,
            CookieContainer = cookies
        };
}
