using Elsa.Workflows.Management;
using Microsoft.Extensions.AI;
using System.ComponentModel;

namespace Elsa.Copilot.Modules.Core.Chat.Tools;

/// <summary>
/// Tool for inspecting a workflow instance's current state.
/// </summary>
public class GetWorkflowInstanceStateTool
{
    private readonly IWorkflowInstanceStore _workflowInstanceStore;

    public GetWorkflowInstanceStateTool(IWorkflowInstanceStore workflowInstanceStore)
    {
        _workflowInstanceStore = workflowInstanceStore;
    }

    [Description("Inspects a running or failed workflow instance's current state")]
    public async Task<object> GetWorkflowInstanceStateAsync(
        [Description("The workflow instance ID to inspect")] string workflowInstanceId,
        CancellationToken cancellationToken = default)
    {
        var instance = await _workflowInstanceStore.FindAsync(workflowInstanceId, cancellationToken);
        
        if (instance == null)
        {
            return new { error = "Workflow instance not found", workflowInstanceId };
        }

        return new
        {
            id = instance.Id,
            definitionId = instance.DefinitionId,
            definitionVersionId = instance.DefinitionVersionId,
            status = instance.Status.ToString(),
            subStatus = instance.SubStatus.ToString(),
            correlationId = instance.CorrelationId,
            createdAt = instance.CreatedAt,
            updatedAt = instance.UpdatedAt,
            finishedAt = instance.FinishedAt,
            faultedAt = instance.FaultedAt,
            workflowState = new
            {
                status = instance.WorkflowState.Status.ToString(),
                subStatus = instance.WorkflowState.SubStatus.ToString(),
                bookmarks = instance.WorkflowState.Bookmarks.Count,
                incidents = instance.WorkflowState.Incidents.Count,
                properties = instance.WorkflowState.Properties.Count
            }
        };
    }
}
