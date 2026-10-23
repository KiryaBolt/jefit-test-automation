namespace Jefit.Test.Support;

public class ScenarioData
{
    public string? RememberedName { get; set; }

    public string? RememberedPath { get; set; }

    public int? InitialCount { get; set; }

    public IReadOnlyList<int> FirstPageIds { get; set; } = Array.Empty<int>();

    public string? CreatedPlanId { get; set; }
}
