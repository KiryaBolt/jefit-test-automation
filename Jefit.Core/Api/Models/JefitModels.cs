namespace Jefit.Core.Api.Models;

/// <summary>Упражнение из базы Jefit: /exercises/{Id}/{Slug}</summary>
public record ExerciseInfo(int Id, string Slug, string Name, string RelativeUrl);

/// <summary>План тренировок из базы Jefit: /routines/{Id}/{Slug}</summary>
public record RoutineInfo(int Id, string Slug, string Name, string RelativeUrl);

/// <summary>SEO-данные страницы из HTML, который отдаёт сервер.</summary>
public record PageMeta(string Title, string? Description, string? OgTitle, string? Canonical, string? H1);
