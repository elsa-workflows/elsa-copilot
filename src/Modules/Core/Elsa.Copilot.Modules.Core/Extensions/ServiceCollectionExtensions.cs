using Elsa.Copilot.Modules.Core.Configuration;
using Elsa.Copilot.Modules.Core.Contracts;
using Elsa.Copilot.Modules.Core.Providers;
using Elsa.Copilot.Modules.Core.Registry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Elsa.Copilot.Modules.Core.Extensions;

/// <summary>
/// Extension methods for configuring AI providers.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds AI provider services to the service collection.
    /// </summary>
    public static IServiceCollection AddAiProviders(this IServiceCollection services, IConfiguration configuration)
    {
        // Register options
        services.Configure<AiProvidersOptions>(configuration.GetSection(AiProvidersOptions.SectionKey));
        
        // Register core services
        services.TryAddSingleton<IAiProviderRegistry, AiProviderRegistry>();
        services.TryAddSingleton<IAiProviderFactory, AiProviderFactory>();

        return services;
    }

    /// <summary>
    /// Adds GitHub Copilot provider to the service collection.
    /// </summary>
    public static IServiceCollection AddGitHubCopilotProvider(this IServiceCollection services, IConfiguration configuration)
    {
        // Register options
        services.Configure<GitHubCopilotOptions>(configuration.GetSection("Elsa:Copilot:AiProviders:GitHubCopilot"));
        
        // Register HTTP client
        services.AddHttpClient("github-copilot", client =>
        {
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        // Register provider
        services.AddSingleton<GitHubCopilotProvider>();
        
        // Register with registry
        services.AddSingleton(sp =>
        {
            var provider = sp.GetRequiredService<GitHubCopilotProvider>();
            var registry = sp.GetRequiredService<IAiProviderRegistry>();
            registry.Register(provider);
            return provider;
        });

        return services;
    }
}
