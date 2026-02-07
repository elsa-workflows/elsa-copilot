using Elsa.Copilot.Modules.Core.Chat.Services;
using Elsa.Copilot.Modules.Core.Chat.Tools;
using Elsa.Features.Abstractions;
using Elsa.Features.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa.Copilot.Modules.Core.Chat;

/// <summary>
/// Feature registration for the Copilot Chat module.
/// </summary>
public class CopilotChatFeature : FeatureBase
{
    public CopilotChatFeature(IModule module) : base(module)
    {
    }

    public override void Configure()
    {
        Module.AddFastEndpointsAssembly(GetType());
    }

    public override void Apply()
    {
        // Register tool functions
        Services.AddScoped<GetWorkflowDefinitionTool>();
        Services.AddScoped<GetActivityCatalogTool>();
        Services.AddScoped<GetWorkflowInstanceStateTool>();
        Services.AddScoped<GetWorkflowInstanceErrorsTool>();

        // Register chat service
        Services.AddScoped<CopilotChatService>();

        // Add controllers for API endpoints
        Services.AddControllers()
            .AddApplicationPart(GetType().Assembly);
    }
}
