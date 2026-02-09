using Elsa.Copilot.Modules.Core.Chat.Services;
using System.ComponentModel;

namespace Elsa.Copilot.Modules.Core.Chat.Tools;

/// <summary>
/// Tool for capturing comprehensive diagnostic snapshots of failed workflows.
/// Phase 4: Diagnostics & Failure Explanation
/// 
/// This tool provides the AI with detailed failure context including:
/// - All incidents/errors with stack traces
/// - Execution timeline showing which activities ran
/// - Current workflow variables and properties
/// - Basic root cause analysis
/// </summary>
public class GetWorkflowDiagnosticsSnapshotTool
{
    private readonly WorkflowDiagnosticsService _diagnosticsService;

    public GetWorkflowDiagnosticsSnapshotTool(WorkflowDiagnosticsService diagnosticsService)
    {
        _diagnosticsService = diagnosticsService;
    }

    [Description("Gets a comprehensive diagnostic snapshot for a failed or problematic workflow instance")]
    public async Task<object> GetDiagnosticsSnapshotAsync(
        [Description("The workflow instance ID to diagnose")] string workflowInstanceId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var snapshot = await _diagnosticsService.CaptureSnapshotAsync(workflowInstanceId, cancellationToken);
            var analysis = _diagnosticsService.AnalyzeSnapshot(snapshot);

            return new
            {
                snapshot = new
                {
                    instanceId = snapshot.InstanceId,
                    definitionId = snapshot.DefinitionId,
                    definitionVersion = snapshot.DefinitionVersion,
                    status = snapshot.Status,
                    failureTimestamp = snapshot.FailureTimestamp,
                    incidents = snapshot.Incidents.Select(i => new
                    {
                        activityId = i.ActivityId,
                        activityType = i.ActivityType,
                        message = i.Message,
                        exception = i.Exception,
                        timestamp = i.Timestamp
                    }),
                    executionHistory = snapshot.ExecutionHistory.Select(e => new
                    {
                        activityId = e.ActivityId,
                        activityType = e.ActivityType,
                        activityName = e.ActivityName,
                        status = e.Status,
                        startedAt = e.StartedAt,
                        completedAt = e.CompletedAt
                    }),
                    variables = snapshot.Variables,
                    properties = snapshot.Properties
                },
                preliminaryAnalysis = new
                {
                    rootCause = analysis.RootCause,
                    suggestedActions = analysis.SuggestedActions
                }
            };
        }
        catch (Exception ex)
        {
            return new
            {
                error = "Failed to capture diagnostic snapshot",
                message = ex.Message,
                workflowInstanceId
            };
        }
    }
}
