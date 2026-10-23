using Jefit.Core.Api.Models;
using RestSharp;

namespace Jefit.Core.Api;

public class JefitWebClient : BaseApiClient
{
    public JefitWebClient() : base(Settings.ApiBaseUrl)
    {
    }

    public Task<ApiResponse> GetPageAsync(string pathOrUrl, bool followRedirects = true)
    {
        var request = new RestRequest(pathOrUrl, Method.Get)
            .AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8")
            .AddHeader("Accept-Language", "en-US,en;q=0.9");

        return SendAsync(request, followRedirects);
    }

    public async Task<IReadOnlyList<ExerciseInfo>> GetExercisesAsync(int page = 1)
    {
        var response = await GetPageAsync($"/exercises?page={page}");
        EnsureSuccess(response, "список упражнений");
        return HtmlPageParser.GetExercises(response.Body);
    }

    public async Task<IReadOnlyList<RoutineInfo>> GetRoutinesAsync(int page = 1)
    {
        var response = await GetPageAsync(page <= 1 ? "/routines" : $"/routines?page={page}");
        EnsureSuccess(response, "список планов");
        return HtmlPageParser.GetRoutines(response.Body);
    }

    private static void EnsureSuccess(ApiResponse response, string what)
    {
        if (!response.IsSuccessful)
        {
            throw new InvalidOperationException(
                $"Не удалось получить {what}: HTTP {response.StatusCode}. {response.Raw.ErrorMessage}");
        }
    }
}
