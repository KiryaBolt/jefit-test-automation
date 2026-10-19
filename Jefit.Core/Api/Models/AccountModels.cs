namespace Jefit.Core.Api.Models;

// Модели запросов аккаунта. Имена полей нужно сверить с реальными запросами
// в DevTools -> Network (RestSharp сериализует их в camelCase: email, password, name).

public record LoginRequest(string Email, string Password);

public record CreateWorkoutPlanRequest(string Name);

public record LoginResult(ApiResponse Response, bool IsSuccess);
