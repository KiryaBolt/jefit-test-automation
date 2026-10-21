using System.Text.Json.Serialization;

namespace Jefit.Core.Api.Models;

// Контракты взяты из реальных запросов сайта (DevTools -> Network и код страницы входа).

/// <summary>Тело POST /api/v2/auth/login. Пароль сайт отправляет не открыто, а как MD5-хеш.</summary>
public record LoginRequest(
    [property: JsonPropertyName("platform")] string Platform,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("passwordMd5")] string PasswordMd5);

/// <summary>Тело POST /api/v2/routines/download: добавить готовый план из каталога в «Мои планы».</summary>
public record DownloadRoutineRequest(
    [property: JsonPropertyName("routine_id")] int RoutineId);

public record LoginResult(ApiResponse Response, bool IsSuccess);
