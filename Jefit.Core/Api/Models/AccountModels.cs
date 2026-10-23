using System.Text.Json.Serialization;

namespace Jefit.Core.Api.Models;

public record LoginRequest(
    [property: JsonPropertyName("platform")] string Platform,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("passwordMd5")] string PasswordMd5);

public record DownloadRoutineRequest(
    [property: JsonPropertyName("routine_id")] int RoutineId);

public record LoginResult(ApiResponse Response, bool IsSuccess);
