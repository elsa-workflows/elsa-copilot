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
        });
        
        // CORS policy for Studio frontend
        svc.AddCors(corsCfg => 
            corsCfg.AddDefaultPolicy(policy => 
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader()));
    }
}
