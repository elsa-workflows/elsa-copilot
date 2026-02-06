using Elsa.Copilot.Core.Security.Extensions;
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
        
        // Register AI Security & Tenancy Guardrails (Feature 24)
        // This provides foundational security layer for AI operations including:
        // - Tenancy authorization via IAiAuthorizationHandler
        // - AI permission scopes (ai:read, ai:propose, ai:diagnose, ai:admin)
        // - Safety gate for tool execution with input/output validation
        // - PII scrubbing and structural validation rules
        svc.AddAiSecurityGuardrails();
        
        // Future modules can be registered here as the architecture evolves
        // Example:
        // svc.AddElsaCopilotAiModule();
        // svc.AddElsaCopilotStudioExtensions();
    }
}
