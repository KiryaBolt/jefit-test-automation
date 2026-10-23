namespace Jefit.Core.Api.Models;

public record ExerciseInfo(int Id, string Slug, string Name, string RelativeUrl);

public record RoutineInfo(int Id, string Slug, string Name, string RelativeUrl);

public record PageMeta(string Title, string? Description, string? OgTitle, string? Canonical, string? H1);
