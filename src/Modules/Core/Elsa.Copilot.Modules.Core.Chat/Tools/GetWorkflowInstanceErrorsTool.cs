using Elsa.Workflows.Management;
using Microsoft.Extensions.AI;
using System.ComponentModel;

namespace Elsa.Copilot.Modules.Core.Chat.Tools;

/// <summary>
/// Tool for retrieving error details for a failed workflow instance.
/// </summary>
public class GetWorkflowInstanceErrorsTool
{
    private readonly IWorkflowInstanceStore _workflowInstanceStore;

    public GetWorkflowInstanceErrorsTool(IWorkflowInstanceStore workflowInstanceStore)
    {
        _workflowInstanceStore = workflowInstanceStore;
    }

    [Description("Gets error details for a failed workflow instance")]
    public async Task<object> GetWorkflowInstanceErrorsAsync(
        [Description("The workflow instance ID to get errors for")] string workflowInstanceId,
        CancellationToken cancellationToken = default)
    {
        var instance = await _workflowInstanceStore.FindAsync(workflowInstanceId, cancellationToken);
        
        if (instance == null)
        {
            return new { error = "Workflow instance not found", workflowInstanceId };
        }

        var incidents = instance.WorkflowState.Incidents.Select(i => new
        {
            activityId = i.ActivityId,
            activityType = i.ActivityType,
            message = i.Message,
            exception = i.Exception,
            timestamp = i.Timestamp
        }).ToList();

        var faults = instance.WorkflowState.ExecutionLog
            .Where(log => log.ActivityType == "Fault" || !string.IsNullOrEmpty(log.Exception))
            .Select(log => new
            {
                activityId = log.ActivityId,
                activityType = log.ActivityType,
                exception = log.Exception,
                timestamp = log.Timestamp
            }).ToList();

        return new
        {
            instanceId = instance.Id,
            status = instance.Status.ToString(),
            faultedAt = instance.FaultedAt,
            incidents = incidents,
            faults = faults,
            totalErrors = incidents.Count + faults.Count
        };
    }
}
