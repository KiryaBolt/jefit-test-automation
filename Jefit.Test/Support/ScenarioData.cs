namespace Jefit.Test.Support;

/// <summary>Данные, которые шаги разных классов передают друг другу внутри одного сценария.</summary>
public class ScenarioData
{
    /// <summary>Название, которое нужно проверить в UI (упражнение, план).</summary>
    public string? RememberedName { get; set; }

    /// <summary>Относительный адрес, который нужно открыть в браузере.</summary>
    public string? RememberedPath { get; set; }

    public int? InitialCount { get; set; }

    public IReadOnlyList<int> FirstPageIds { get; set; } = Array.Empty<int>();

    public string? CreatedPlanId { get; set; }
}
