namespace Elsa.Copilot.Modules.Core.Chat.Models;

/// <summary>
/// Represents a comprehensive snapshot of workflow state at the time of failure.
/// Captures all relevant context for AI-powered diagnostic analysis.
/// </summary>
public class DiagnosticSnapshot
{
    /// <summary>
    /// The workflow instance ID
    /// </summary>
    public string InstanceId { get; set; } = string.Empty;

    /// <summary>
    /// The workflow definition ID
    /// </summary>
    public string DefinitionId { get; set; } = string.Empty;

    /// <summary>
    /// The workflow definition version
    /// </summary>
    public int DefinitionVersion { get; set; }

    /// <summary>
    /// Current workflow status (e.g., Faulted, Cancelled)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Detailed incident/error information
    /// </summary>
    public List<DiagnosticIncident> Incidents { get; set; } = new();

    /// <summary>
    /// Workflow execution timeline - sequence of activity executions
    /// </summary>
    public List<ActivityExecutionEntry> ExecutionHistory { get; set; } = new();

    /// <summary>
    /// Current workflow variables and their values
    /// </summary>
    public Dictionary<string, object?> Variables { get; set; } = new();

    /// <summary>
    /// Workflow instance properties
    /// </summary>
    public Dictionary<string, object?> Properties { get; set; } = new();

    /// <summary>
    /// Timestamp when the failure occurred
    /// </summary>
    public DateTimeOffset FailureTimestamp { get; set; }

    /// <summary>
    /// Suggested root cause analysis (can be populated by AI)
    /// </summary>
    public string? RootCauseAnalysis { get; set; }

    /// <summary>
    /// Suggested next actions (can be populated by AI)
    /// </summary>
    public List<string> SuggestedActions { get; set; } = new();
}

/// <summary>
/// Represents a single incident/error in the workflow
/// </summary>
public class DiagnosticIncident
{
    /// <summary>
    /// The activity ID where the error occurred
    /// </summary>
    public string ActivityId { get; set; } = string.Empty;

    /// <summary>
    /// The activity type (e.g., "HttpActivity", "SqlActivity")
    /// </summary>
    public string ActivityType { get; set; } = string.Empty;

    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Exception details including stack trace
    /// </summary>
    public string? Exception { get; set; }

    /// <summary>
    /// When the incident occurred
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }
}

/// <summary>
/// Represents a single activity execution in the timeline
/// </summary>
public class ActivityExecutionEntry
{
    /// <summary>
    /// Activity ID
    /// </summary>
    public string ActivityId { get; set; } = string.Empty;

    /// <summary>
    /// Activity type
    /// </summary>
    public string ActivityType { get; set; } = string.Empty;

    /// <summary>
    /// Activity name (if available)
    /// </summary>
    public string? ActivityName { get; set; }

    /// <summary>
    /// Execution status (e.g., "Completed", "Faulted", "Cancelled")
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// When the activity started
    /// </summary>
    public DateTimeOffset? StartedAt { get; set; }

    /// <summary>
    /// When the activity completed
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }
}
