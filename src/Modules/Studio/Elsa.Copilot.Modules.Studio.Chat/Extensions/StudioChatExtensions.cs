using Elsa.Copilot.Modules.Studio.Chat.Services;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace Elsa.Copilot.Modules.Studio.Chat.Extensions;

/// <summary>
/// Extension methods for registering the Studio Chat module.
/// </summary>
public static class StudioChatExtensions
{
    /// <summary>
    /// Adds the Studio Chat UI module to the service collection.
    /// This registers MudBlazor services and the chat components.
    /// </summary>
    public static IServiceCollection AddStudioChatModule(this IServiceCollection services)
    {
        // Register MudBlazor services
        services.AddMudServices();
        
        // Register chat services
        services.AddScoped<ChatSessionState>();
        
        // Register HttpClient for StudioChatClient
        // In Blazor Server, HttpClient is configured at the scoped level and needs base address
        services.AddScoped<StudioChatClient>();
        services.AddHttpClient();
        
        return services;
    }
}
