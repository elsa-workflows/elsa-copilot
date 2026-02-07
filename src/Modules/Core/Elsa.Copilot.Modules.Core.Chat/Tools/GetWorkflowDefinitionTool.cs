using Elsa.Common.Models;
using Elsa.Workflows.Management;
using Elsa.Workflows.Management.Filters;
using Microsoft.Extensions.AI;
using System.ComponentModel;

namespace Elsa.Copilot.Modules.Core.Chat.Tools;

/// <summary>
/// Tool for retrieving workflow definition structure and metadata.
/// </summary>
public class GetWorkflowDefinitionTool
{
    private readonly IWorkflowDefinitionStore _workflowDefinitionStore;

    public GetWorkflowDefinitionTool(IWorkflowDefinitionStore workflowDefinitionStore)
    {
        _workflowDefinitionStore = workflowDefinitionStore;
    }

    [Description("Retrieves a workflow definition's metadata by ID")]
    public async Task<object> GetWorkflowDefinitionAsync(
        [Description("The workflow definition ID to retrieve")] string workflowDefinitionId,
        CancellationToken cancellationToken = default)
    {
        var filter = new WorkflowDefinitionFilter
        {
            DefinitionId = workflowDefinitionId,
            VersionOptions = VersionOptions.Published
        };

        var definition = await _workflowDefinitionStore.FindAsync(filter, cancellationToken);
        
        if (definition == null)
        {
            return new { error = "Workflow definition not found", workflowDefinitionId };
        }

        return new
        {
            id = definition.Id,
            definitionId = definition.DefinitionId,
            version = definition.Version,
            name = definition.Name,
            description = definition.Description,
            isPublished = definition.IsPublished,
            isLatest = definition.IsLatest,
            createdAt = definition.CreatedAt,
            materializer = definition.MaterializerName,
            // Include the serialized workflow structure
            stringData = definition.StringData
        };
    }
}
