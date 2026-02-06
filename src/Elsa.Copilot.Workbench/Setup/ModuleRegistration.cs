using Elsa.Copilot.Modules.Core.Placeholder;
using Elsa.Copilot.Modules.Studio.Placeholder;

namespace Elsa.Copilot.Workbench.Setup;

/// <summary>
/// Registers custom Elsa Copilot modules for both server-side and studio-side functionality.
/// As modules evolve from placeholders to full implementations, their registration
/// will expand to include services, activities, and UI components.
/// </summary>
internal static class ModuleRegistration
{
    internal static void RegisterModules(IServiceCollection svc)
    {
        // Register Core module (server-side)
        // Currently a placeholder - will contain activities, services, and AI integrations
        svc.AddSingleton<CoreModulePlaceholder>();
        
        // Register Studio module (UI-side)
        // Currently a placeholder - will contain Blazor components and UI extensions
        svc.AddSingleton<StudioModulePlaceholder>();
        
        // Future modules can be registered here as the architecture evolves
        // Example:
        // svc.AddElsaCopilotAiModule();
        // svc.AddElsaCopilotStudioExtensions();
    }
}
