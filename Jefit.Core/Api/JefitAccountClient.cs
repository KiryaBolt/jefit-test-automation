using System.Net;
using System.Text.Json;
using Jefit.Core.Api.Models;
using Jefit.Core.Infrastructure;
using RestSharp;

namespace Jefit.Core.Api;

/// <summary>
/// Клиент для авторизованных запросов (логин, планы тренировок).
/// ВАЖНО: у Jefit нет публичного API. Пути и формат запросов настраиваются в appsettings.json
/// после изучения сайта в DevTools -> Network. Пока не настроено, тесты @auth пропускаются.
/// </summary>
public class JefitAccountClient : BaseApiClient
{
    private static readonly string[] TokenFields = { "token", "accessToken", "access_token", "jwt" };
    private static readonly string[] IdFields = { "id", "routineId", "planId", "routine_id" };

    public JefitAccountClient() : this(new CookieContainer())
    {
    }

    private JefitAccountClient(CookieContainer cookies) : base(GetBaseUrl(), cookies)
    {
        Cookies = cookies;
    }

    /// <summary>Cookie сессии после логина — их можно перенести в браузер.</summary>
    public CookieContainer Cookies { get; }

    public string? AccessToken { get; private set; }

    public async Task<LoginResult> LoginAsync(UserCredentials credentials)
    {
        var request = new RestRequest(Settings.AccountLoginPath, Method.Post)
            .AddJsonBody(new LoginRequest(credentials.Email, credentials.Password));

        var response = await SendAsync(request);
        AccessToken = FindValue(response.Body, TokenFields);

        var hasSession = AccessToken is not null || Cookies.Count > 0;
        return new LoginResult(response, response.IsSuccessful && hasSession);
    }

    public async Task<string> CreateWorkoutPlanAsync(string name)
    {
        var request = Authorize(new RestRequest(Settings.AccountRoutinesPath, Method.Post)
            .AddJsonBody(new CreateWorkoutPlanRequest(name)));

        var response = await SendAsync(request);
        if (!response.IsSuccessful)
        {
            throw new InvalidOperationException($"План не создан: HTTP {response.StatusCode}");
        }

        return FindValue(response.Body, IdFields)
               ?? throw new InvalidOperationException("В ответе нет id созданного плана.");
    }

    public Task<ApiResponse> DeleteWorkoutPlanAsync(string id) =>
        SendAsync(Authorize(new RestRequest($"{Settings.AccountRoutinesPath.TrimEnd('/')}/{id}", Method.Delete)));

    private RestRequest Authorize(RestRequest request)
    {
        if (AccessToken is not null)
        {
            request.AddHeader("Authorization", $"Bearer {AccessToken}");
        }

        return request;
    }

    private static string GetBaseUrl() =>
        string.IsNullOrWhiteSpace(Settings.AccountApiBaseUrl) ? Settings.ApiBaseUrl : Settings.AccountApiBaseUrl;

    /// <summary>Рекурсивно ищет в JSON первое поле с одним из имён (токен, id).</summary>
    private static string? FindValue(string json, IReadOnlyCollection<string> names)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            using var document = JsonDocument.Parse(json);
            return Find(document.RootElement);
        }
        catch (JsonException)
        {
            return null;
        }

        string? Find(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in element.EnumerateObject())
                {
                    if (names.Contains(property.Name, StringComparer.OrdinalIgnoreCase) &&
                        property.Value.ValueKind is JsonValueKind.String or JsonValueKind.Number)
                    {
                        return property.Value.ToString();
                    }

                    var nested = Find(property.Value);
                    if (nested is not null) return nested;
                }
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in element.EnumerateArray())
                {
                    var nested = Find(item);
                    if (nested is not null) return nested;
                }
            }

            return null;
        }
    }
}
