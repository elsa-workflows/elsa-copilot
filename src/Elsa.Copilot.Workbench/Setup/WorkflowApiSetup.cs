using Elsa.Extensions;

namespace Elsa.Copilot.Workbench.Setup;

/// <summary>
/// Configures HTTP-based workflow activities and REST API endpoints
/// for the Copilot workbench server.
/// </summary>
internal static class WorkflowApiSetup
{
    internal static void AddHttpAndApi(IServiceCollection svc, IConfiguration cfg)
    {
        svc.AddElsa(elsaCfg =>
        {
            // REST API for workflow management
            elsaCfg.UseWorkflowsApi();
            
            // NOTE: UseHttp() intentionally omitted - caused middleware conflicts
            // Will be re-enabled when routing is properly configured
        });
        
        // CORS policy for Studio frontend
        // NOTE: Permissive for development workbench - restrict in production
        svc.AddCors(corsCfg => 
            corsCfg.AddDefaultPolicy(policy => 
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader()));
    }
}
