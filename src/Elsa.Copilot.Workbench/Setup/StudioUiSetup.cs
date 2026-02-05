using Elsa.Studio.Core.BlazorServer.Extensions;
using Elsa.Studio.Dashboard.Extensions;
using Elsa.Studio.Extensions;
using Elsa.Studio.Login.BlazorServer.Extensions;
using Elsa.Studio.Login.HttpMessageHandlers;
using Elsa.Studio.Models;
using Elsa.Studio.Shell.Extensions;
using Elsa.Studio.Workflows.Extensions;

namespace Elsa.Copilot.Workbench.Setup;

/// <summary>
/// Configures the Blazor Server UI components for Elsa Studio within
/// the Copilot workbench, including the dashboard, workflow designer, and login.
/// </summary>
internal static class StudioUiSetup
{
    internal static void AddStudioModules(IServiceCollection svc, IConfiguration cfg)
    {
        // Blazor Server infrastructure
        svc.AddRazorPages();
        svc.AddServerSideBlazor();
        
        // Studio backend API configuration (no auth for now)
        var apiCfg = new BackendApiConfig
        {
            ConfigureBackendOptions = opts => cfg.GetSection("Backend").Bind(opts)
        };
        
        // Studio core modules
        svc.AddCore();
        svc.AddShell();
        svc.AddRemoteBackend(apiCfg);
        svc.AddDashboardModule();
        svc.AddWorkflowsModule();
    }
}
