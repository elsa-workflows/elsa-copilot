using Elsa.Studio.Core.BlazorServer.Extensions;
using Elsa.Studio.Dashboard.Extensions;
using Elsa.Studio.Extensions;
using Elsa.Studio.Login.BlazorServer.Extensions;
using Elsa.Studio.Models;
using Elsa.Studio.Shell.Extensions;
using Elsa.Studio.Workflows.Extensions;
using Elsa.Copilot.Modules.Studio.Chat.Extensions;

namespace Elsa.Copilot.Workbench.Setup;

/// <summary>
/// Configures Elsa Studio (Blazor Server UI) services including the dashboard,
/// workflow designer, shell, and backend API connection.
/// </summary>
internal static class ElsaStudioSetup
{
    internal static void AddElsaStudio(IServiceCollection svc, IConfiguration cfg)
    {
        // Blazor Server infrastructure
        svc.AddRazorPages();
        svc.AddServerSideBlazor(options =>
        {
            // Increase circuit size for complex workflow diagrams
            options.DetailedErrors = true;
        });
        
        // Studio backend API configuration
        // For development, using simple configuration without authentication
        var apiCfg = new BackendApiConfig
        {
            ConfigureBackendOptions = opts => cfg.GetSection("Backend").Bind(opts)
        };
        
        // Studio core services
        svc.AddCore();
        svc.AddShell();
        svc.AddRemoteBackend(apiCfg);
        
        // Studio feature modules
        svc.AddDashboardModule();
        svc.AddWorkflowsModule();
        
        // Copilot Chat UI module
        svc.AddStudioChatModule();
        
        // Login module for authentication (when needed)
        // Currently disabled for simplified development setup
        // svc.AddLoginModule();
    }
}
