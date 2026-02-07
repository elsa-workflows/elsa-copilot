using Elsa.Workflows;
using Microsoft.Extensions.AI;
using System.ComponentModel;

namespace Elsa.Copilot.Modules.Core.Chat.Tools;

/// <summary>
/// Tool for listing available activity types and their schemas.
/// </summary>
public class GetActivityCatalogTool
{
    private readonly IActivityRegistry _activityRegistry;

    public GetActivityCatalogTool(IActivityRegistry activityRegistry)
    {
        _activityRegistry = activityRegistry;
    }

    [Description("Lists all available activity types and their schemas")]
    public Task<object> GetActivityCatalogAsync(
        [Description("Optional category filter (e.g., 'Workflows', 'Http', 'Scheduling')")] string? category = null,
        CancellationToken cancellationToken = default)
    {
        var activities = _activityRegistry.List();

        if (!string.IsNullOrWhiteSpace(category))
        {
            activities = activities.Where(a => 
                a.Category?.Equals(category, StringComparison.OrdinalIgnoreCase) == true);
        }

        var catalog = activities.Select(a => new
        {
            type = a.TypeName,
            name = a.Name,
            displayName = a.DisplayName,
            description = a.Description,
            category = a.Category,
            inputs = a.Inputs.Select(i => new
            {
                name = i.Name,
                displayName = i.DisplayName,
                description = i.Description,
                type = i.Type.Name
            }),
            outputs = a.Outputs.Select(o => new
            {
                name = o.Name,
                displayName = o.DisplayName,
                description = o.Description,
                type = o.Type.Name
            })
        }).ToList();

        return Task.FromResult<object>(new { activities = catalog, count = catalog.Count });
    }
}
