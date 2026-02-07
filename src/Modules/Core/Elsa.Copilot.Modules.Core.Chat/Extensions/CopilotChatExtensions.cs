using Elsa.Copilot.Modules.Core.Chat.Services;
using Elsa.Copilot.Modules.Core.Chat.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Elsa.Extensions;

/// <summary>
/// Extension methods for registering the Copilot Chat module.
/// </summary>
public static class CopilotChatExtensions
{
    /// <summary>
    /// Adds the Copilot Chat services to the service collection.
    /// Use this in conjunction with AddElsa to register the chat module.
    /// </summary>
    public static IServiceCollection AddCopilotChat(this IServiceCollection services)
    {
        // Register tool functions
        services.AddScoped<GetWorkflowDefinitionTool>();
        services.AddScoped<GetActivityCatalogTool>();
        services.AddScoped<GetWorkflowInstanceStateTool>();
        services.AddScoped<GetWorkflowInstanceErrorsTool>();

        // Register mock chat client only if no IChatClient is already registered
        // This allows the host to register a real AI provider before calling AddCopilotChat()
        // Example: services.AddSingleton<IChatClient>(sp => new AzureOpenAIClient(...).AsChatClient("gpt-4"))
        services.TryAddSingleton<IChatClient, MockChatClient>();

        // Register chat service
        services.AddScoped<CopilotChatService>();

        // Add controllers for API endpoints
        services.AddControllers()
            .AddApplicationPart(typeof(CopilotChatExtensions).Assembly);

        return services;
    }
}
