using Elsa.Copilot.Modules.Core.Chat.Models;
using Elsa.Extensions;
using Elsa.Workflows.Management;
using System.Text.Json;

namespace Elsa.Copilot.Modules.Core.Chat.Services;

/// <summary>
/// Service for capturing workflow diagnostic snapshots and performing failure analysis.
/// Phase 4: Diagnostics & Failure Explanation
/// 
/// This service builds comprehensive diagnostic snapshots of failed workflows,
/// capturing incidents, workflow state, and context needed for AI analysis.
/// </summary>
public class WorkflowDiagnosticsService
{
    private readonly IWorkflowInstanceStore _workflowInstanceStore;
    private readonly IWorkflowDefinitionStore _workflowDefinitionStore;

    public WorkflowDiagnosticsService(
        IWorkflowInstanceStore workflowInstanceStore,
        IWorkflowDefinitionStore workflowDefinitionStore)
    {
        _workflowInstanceStore = workflowInstanceStore;
        _workflowDefinitionStore = workflowDefinitionStore;
    }

    /// <summary>
    /// Captures a comprehensive diagnostic snapshot for a workflow instance.
    /// This includes incidents, execution state, variables, and properties.
    /// </summary>
    public async Task<DiagnosticSnapshot> CaptureSnapshotAsync(
        string workflowInstanceId, 
        CancellationToken cancellationToken = default)
    {
        var instance = await _workflowInstanceStore.FindAsync(workflowInstanceId, cancellationToken);
        
        if (instance == null)
        {
            throw new InvalidOperationException($"Workflow instance '{workflowInstanceId}' not found");
        }

        var snapshot = new DiagnosticSnapshot
        {
            InstanceId = instance.Id,
            DefinitionId = instance.DefinitionId,
            DefinitionVersion = instance.Version,
            Status = instance.Status.ToString(),
            FailureTimestamp = DateTimeOffset.UtcNow
        };

        // Capture incidents/errors
        snapshot.Incidents = instance.WorkflowState.Incidents.Select(i => new DiagnosticIncident
        {
            ActivityId = i.ActivityId,
            ActivityType = i.ActivityType,
            Message = i.Message,
            Exception = SerializeException(i.Exception),
            Timestamp = i.Timestamp
        }).ToList();

        // Note: Detailed execution history is not directly available from WorkflowState
        // The execution timeline can be inferred from the incidents and workflow status

        // Capture workflow variables
        // Variables are stored in the workflow state's properties
        var properties = instance.WorkflowState.Properties ?? new Dictionary<string, object>();
        snapshot.Variables = properties.ToDictionary(kvp => kvp.Key, kvp => (object?)kvp.Value);

        // Capture workflow instance properties
        snapshot.Properties = new Dictionary<string, object?>
        {
            ["CorrelationId"] = instance.CorrelationId,
            ["CreatedAt"] = instance.CreatedAt,
            ["UpdatedAt"] = instance.UpdatedAt,
            ["FinishedAt"] = instance.FinishedAt
        };

        return snapshot;
    }

    /// <summary>
    /// Serializes an exception state to a string for diagnostic purposes
    /// </summary>
    private string? SerializeException(object? exception)
    {
        if (exception == null)
            return null;

        try
        {
            return JsonSerializer.Serialize(exception, new JsonSerializerOptions { WriteIndented = true });
        }
        catch
        {
            return exception.ToString();
        }
    }

    /// <summary>
    /// Analyzes a diagnostic snapshot to identify root cause and suggest actions.
    /// This is a basic heuristic analysis - the AI will provide deeper insights.
    /// </summary>
    public DiagnosticAnalysis AnalyzeSnapshot(DiagnosticSnapshot snapshot)
    {
        var analysis = new DiagnosticAnalysis
        {
            InstanceId = snapshot.InstanceId,
            RootCause = DetermineRootCause(snapshot),
            SuggestedActions = GenerateSuggestedActions(snapshot)
        };

        return analysis;
    }

    private string DetermineRootCause(DiagnosticSnapshot snapshot)
    {
        if (!snapshot.Incidents.Any())
        {
            return "No incidents found. The workflow may have been cancelled or stopped.";
        }

        var firstIncident = snapshot.Incidents.OrderBy(i => i.Timestamp).First();
        
        // Basic pattern matching on common errors
        if (firstIncident.Exception?.Contains("timeout", StringComparison.OrdinalIgnoreCase) == true)
        {
            return $"Timeout error in activity '{firstIncident.ActivityType}' (ID: {firstIncident.ActivityId})";
        }
        
        if (firstIncident.Exception?.Contains("connection", StringComparison.OrdinalIgnoreCase) == true ||
            firstIncident.Exception?.Contains("network", StringComparison.OrdinalIgnoreCase) == true)
        {
            return $"Network/connection error in activity '{firstIncident.ActivityType}' (ID: {firstIncident.ActivityId})";
        }
        
        if (firstIncident.Exception?.Contains("null", StringComparison.OrdinalIgnoreCase) == true)
        {
            return $"Null reference error in activity '{firstIncident.ActivityType}' (ID: {firstIncident.ActivityId})";
        }

        return $"Error in activity '{firstIncident.ActivityType}' (ID: {firstIncident.ActivityId}): {firstIncident.Message}";
    }

    private List<string> GenerateSuggestedActions(DiagnosticSnapshot snapshot)
    {
        var actions = new List<string>();

        if (!snapshot.Incidents.Any())
        {
            actions.Add("Review workflow execution logs to understand why the workflow stopped");
            return actions;
        }

        var firstIncident = snapshot.Incidents.OrderBy(i => i.Timestamp).First();

        // Add generic suggestions
        actions.Add($"Review the configuration of activity '{firstIncident.ActivityType}' (ID: {firstIncident.ActivityId})");
        actions.Add("Check if all required inputs and variables are properly set");
        
        // Add specific suggestions based on error patterns
        if (firstIncident.Exception?.Contains("timeout", StringComparison.OrdinalIgnoreCase) == true)
        {
            actions.Add("Increase timeout settings for the activity");
            actions.Add("Check if the target service is responding");
        }
        
        if (firstIncident.Exception?.Contains("connection", StringComparison.OrdinalIgnoreCase) == true)
        {
            actions.Add("Verify network connectivity to the target service");
            actions.Add("Check connection string or endpoint configuration");
        }
        
        if (firstIncident.Exception?.Contains("null", StringComparison.OrdinalIgnoreCase) == true)
        {
            actions.Add("Ensure all required variables are initialized before this activity");
            actions.Add("Add null checks or default values");
        }

        actions.Add("Use the Copilot chat to get detailed analysis and recommendations");
        
        return actions;
    }
}

/// <summary>
/// Result of diagnostic analysis
/// </summary>
public class DiagnosticAnalysis
{
    public string InstanceId { get; set; } = string.Empty;
    public string RootCause { get; set; } = string.Empty;
    public List<string> SuggestedActions { get; set; } = new();
}
