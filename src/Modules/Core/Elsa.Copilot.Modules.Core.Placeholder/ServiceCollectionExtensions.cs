using Elsa.Copilot.Modules.Core.Placeholder.Abstractions;
using Elsa.Copilot.Modules.Core.Placeholder.Models;
using Elsa.Copilot.Modules.Core.Placeholder.Providers.GitHubCopilot;
using Elsa.Copilot.Modules.Core.Placeholder.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa.Copilot.Modules.Core.Placeholder;

/// <summary>
/// Extension methods for registering AI provider services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the AI provider abstraction layer with the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAiProviders(this IServiceCollection services, IConfiguration configuration)
    {
        // Register configuration
        services.Configure<AiProviderOptions>(configuration.GetSection("AiProviders"));

        // Register core services
        services.AddSingleton<IAiProviderRegistry, AiProviderRegistry>();
        services.AddSingleton<IAiProviderFactory, AiProviderFactory>();

        // Register GitHub Copilot provider
        services.AddTransient<GitHubCopilotClient>();
        services.AddSingleton<IAiProvider, GitHubCopilotProvider>();

        // Register the provider with the registry
        services.AddSingleton(sp =>
        {
            var registry = sp.GetRequiredService<IAiProviderRegistry>();
            var providers = sp.GetServices<IAiProvider>();
            
            foreach (var provider in providers)
            {
                registry.Register(provider);
            }

            return registry;
        });

        return services;
    }
}
