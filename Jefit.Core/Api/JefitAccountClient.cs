using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Jefit.Core.Api.Models;
using Jefit.Core.Infrastructure;
using RestSharp;

namespace Jefit.Core.Api;

public class JefitAccountClient : BaseApiClient
{
    public JefitAccountClient() : this(new CookieContainer())
    {
    }

    private JefitAccountClient(CookieContainer cookies) : base(GetBaseUrl(), cookies)
    {
        Cookies = cookies;
    }

    public CookieContainer Cookies { get; }

    public string? AccessToken { get; private set; }

    public async Task<LoginResult> LoginAsync(UserCredentials credentials)
    {
        var request = new RestRequest(Settings.AccountLoginPath, Method.Post)
            .AddJsonBody(new LoginRequest("web", credentials.Email, Md5Hex(credentials.Password)));

        var response = await SendAsync(request);
        AccessToken = ReadString(response.Body, "accessToken");

        var hasSession = AccessToken is not null || Cookies.Count > 0;
        return new LoginResult(response, response.IsSuccessful && hasSession);
    }

    public async Task<string> AddRoutineFromCatalogAsync(int catalogRoutineId)
    {
        var request = new RestRequest($"{Settings.AccountRoutinesPath.TrimEnd('/')}/download", Method.Post)
            .AddJsonBody(new DownloadRoutineRequest(catalogRoutineId));

        var response = await SendAsync(request);
        if (!response.IsSuccessful)
        {
            throw new InvalidOperationException($"План {catalogRoutineId} не добавлен: HTTP {response.StatusCode}");
        }

        var rowId = ReadNumber(response.Body, "row_id")
                    ?? throw new InvalidOperationException("В ответе нет row_id добавленного плана.");

        return $"u_{rowId:D12}";
    }

    public Task<ApiResponse> DeleteRoutineAsync(string userRoutineId) =>
        SendAsync(new RestRequest($"{Settings.AccountRoutinesPath.TrimEnd('/')}/{userRoutineId}", Method.Delete));

    private static string GetBaseUrl() =>
        string.IsNullOrWhiteSpace(Settings.AccountApiBaseUrl) ? Settings.ApiBaseUrl : Settings.AccountApiBaseUrl;

    private static string Md5Hex(string value) =>
        Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    private static string? ReadString(string json, string property)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            return document.RootElement.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String
                ? value.GetString()
                : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static long? ReadNumber(string json, string property)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            if (!document.RootElement.TryGetProperty(property, out var value)) return null;
            if (value.ValueKind == JsonValueKind.Number && value.TryGetInt64(out var number)) return number;
            return value.ValueKind == JsonValueKind.String && long.TryParse(value.GetString(), out var parsed) ? parsed : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
