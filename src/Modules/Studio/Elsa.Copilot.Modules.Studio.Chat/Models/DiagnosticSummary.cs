namespace Elsa.Copilot.Modules.Studio.Chat.Models;

/// <summary>
/// Diagnostic summary model for displaying in the Studio UI
/// </summary>
public class DiagnosticSummary
{
    public string InstanceId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string RootCause { get; set; } = string.Empty;
    public List<string> SuggestedActions { get; set; } = new();
    public List<IncidentSummary> Incidents { get; set; } = new();
    public DateTimeOffset FailureTimestamp { get; set; }
}

/// <summary>
/// Summary of a single incident for UI display
/// </summary>
public class IncidentSummary
{
    public string ActivityId { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
}
