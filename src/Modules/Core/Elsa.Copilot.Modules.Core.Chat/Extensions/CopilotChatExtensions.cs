using Elsa.Copilot.Modules.Core.Chat;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Elsa.Extensions;

/// <summary>
/// Extension methods for registering the Copilot Chat module.
/// </summary>
public static class CopilotChatExtensions
{
    /// <summary>
    /// Adds the Copilot Chat feature to Elsa.
    /// </summary>
    public static IServiceCollection AddCopilotChat(this IServiceCollection services)
    {
        return services.AddFeature<CopilotChatFeature>();
    }
}
