using Elsa.Copilot.Modules.Core.Extensions;
using Elsa.Copilot.Modules.Studio.Placeholder;

namespace Elsa.Copilot.Workbench.Setup;

/// <summary>
/// Registers custom Elsa Copilot modules for both server-side and studio-side functionality.
/// As modules evolve from placeholders to full implementations, their registration
/// will expand to include services, activities, and UI components.
/// </summary>
internal static class ModuleRegistration
{
    internal static void RegisterModules(IServiceCollection services, IConfiguration configuration)
    {
        // Register Core module (server-side)
        // Register AI provider abstraction layer
        services.AddAiProviders(configuration);
        services.AddGitHubCopilotProvider(configuration);
        
        // Register Studio module (UI-side)
        // Currently a placeholder - will contain Blazor components and UI extensions
        services.AddSingleton<StudioModulePlaceholder>();
        
        // Future modules can be registered here as the architecture evolves
        // Example:
        // svc.AddElsaCopilotAiModule();
        // svc.AddElsaCopilotStudioExtensions();
    }
}
