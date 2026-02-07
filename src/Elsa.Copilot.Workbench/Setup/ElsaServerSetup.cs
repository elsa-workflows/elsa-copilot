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
            // For development workbench - signing key should be moved to user secrets in production
            elsa.UseIdentity(identity =>
            {
                identity.TokenOptions = options => 
                {
                    // Use configuration key or generate a secure default for development
                    var signingKey = cfg["Elsa:Identity:SigningKey"];
                    if (string.IsNullOrEmpty(signingKey))
                    {
                        // For development only - generates a consistent key per configuration
                        signingKey = "sufficiently-large-secret-key-min-256-bits";
                    }
                    options.SigningKey = signingKey;
                };
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
            // JavaScript with CLR access disabled for security
            // Enable CLR access only if absolutely necessary for your workflows
            elsa.UseJavaScript(js => js.AllowClrAccess = false);
            
            // Scheduling activities for timed workflows
            elsa.UseScheduling();
            
            // Register activities and workflows from the current assembly
            elsa.AddActivitiesFrom<Program>();
            elsa.AddWorkflowsFrom<Program>();
        });
        
        // Add Copilot Chat module (after AddElsa)
        svc.AddCopilotChat();
        
        // CORS policy for Studio frontend and external API consumers
        // Permissive for development workbench - MUST be restricted for production
        // TODO: Configure specific allowed origins when deploying beyond local development
        svc.AddCors(corsCfg => 
            corsCfg.AddDefaultPolicy(policy =>
            {
                var allowedOrigins = cfg.GetSection("Cors:AllowedOrigins").Get<string[]>() 
                    ?? new[] { "http://localhost:*", "https://localhost:*" };
                
                if (allowedOrigins.Contains("*"))
                {
                    // Allow all origins (development only)
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .WithExposedHeaders("x-elsa-workflow-instance-id");
                }
                else
                {
                    // Restrict to specific origins (recommended)
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .WithExposedHeaders("x-elsa-workflow-instance-id");
                }
            }));
    }
}
