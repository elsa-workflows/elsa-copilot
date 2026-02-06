using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;

namespace Elsa.Copilot.Workbench.Setup;

/// <summary>
/// Configures Elsa Server services including workflow management, runtime execution,
/// HTTP activities, API endpoints, and authentication.
/// </summary>
internal static class ElsaServerSetup
{
    internal static void AddElsaServer(IServiceCollection svc, IConfiguration cfg)
    {
        var sqliteConn = cfg.GetConnectionString("Sqlite") ?? "Data Source=copilot.db;Cache=Shared";
        
        svc.AddElsa(elsa =>
        {
            // Workflow definitions management database
            elsa.UseWorkflowManagement(mgmt => 
                mgmt.UseEntityFrameworkCore(db => db.UseSqlite(sqliteConn)));
            
            // Workflow execution state database
            elsa.UseWorkflowRuntime(rt => 
                rt.UseEntityFrameworkCore(db => db.UseSqlite(sqliteConn)));
            
            // Identity and authentication configuration
            // For now, uses simple API key authentication suitable for development
            elsa.UseIdentity(identity =>
            {
                identity.TokenOptions = options => options.SigningKey = "sufficiently-large-secret-key-min-256-bits";
                identity.UseAdminUserProvider();
            });
            
            elsa.UseDefaultAuthentication(auth => auth.UseAdminApiKey());
            
            // REST API for workflow management
            elsa.UseWorkflowsApi();
            
            // HTTP activities for workflows
            elsa.UseHttp(http =>
            {
                http.ConfigureHttpOptions = options =>
                {
                    options.BaseUrl = new Uri(cfg["Http:BaseUrl"] ?? "https://localhost:5001");
                    options.BasePath = cfg["Http:BasePath"] ?? "/workflows";
                };
            });
            
            // C# and JavaScript expression support
            elsa.UseCSharp();
            elsa.UseJavaScript(js => js.AllowClrAccess = true);
            
            // Scheduling activities for timed workflows
            elsa.UseScheduling();
            
            // Register activities and workflows from the current assembly
            elsa.AddActivitiesFrom<Program>();
            elsa.AddWorkflowsFrom<Program>();
        });
        
        // CORS policy for Studio frontend and external API consumers
        // NOTE: Permissive for development workbench - restrict in production
        svc.AddCors(corsCfg => 
            corsCfg.AddDefaultPolicy(policy => 
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .WithExposedHeaders("x-elsa-workflow-instance-id")));
    }
}
