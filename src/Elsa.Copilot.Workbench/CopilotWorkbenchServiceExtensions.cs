using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;

namespace Elsa.Copilot.Workbench;

/// <summary>
/// Extension methods for configuring the Copilot Workbench services.
/// This encapsulates the specific configuration needed for the hybrid Elsa Server + Studio setup.
/// </summary>
public static class CopilotWorkbenchServiceExtensions
{
    /// <summary>
    /// Adds and configures all services required for the Copilot Workbench,
    /// including Elsa workflow engine, persistence, Studio UI, and APIs.
    /// </summary>
    public static IServiceCollection AddCopilotWorkbench(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Configure the data store for workflows and runtime state
        ConfigureWorkflowPersistence(services, configuration);
        
        // Configure authentication and security
        ConfigureAuthentication(services, configuration);
        
        // Configure the Blazor Server UI components
        ConfigureBlazorStudio(services, configuration);
        
        // Configure API endpoints
        ConfigureApiEndpoints(services, configuration);
        
        return services;
    }
    
    private static void ConfigureWorkflowPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var dbConnectionString = configuration.GetConnectionString("Sqlite") 
            ?? "Data Source=copilot-workflows.db;Cache=Shared";
            
        services.AddElsa(elsa =>
        {
            // Configure workflow management store
            elsa.UseWorkflowManagement(mgmt =>
            {
                mgmt.UseEntityFrameworkCore(ef => ef.UseSqlite(dbConnectionString));
            });
            
            // Configure workflow runtime store
            elsa.UseWorkflowRuntime(runtime =>
            {
                runtime.UseEntityFrameworkCore(ef => ef.UseSqlite(dbConnectionString));
            });
            
            // Scan for activities and workflows in this assembly
            elsa.AddActivitiesFrom<Program>();
            elsa.AddWorkflowsFrom<Program>();
        });
    }
    
    private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        // Placeholder for Identity configuration
    }
    
    private static void ConfigureBlazorStudio(IServiceCollection services, IConfiguration configuration)
    {
        // Placeholder for Blazor configuration
    }
    
    private static void ConfigureApiEndpoints(IServiceCollection services, IConfiguration configuration)
    {
        // Placeholder for API configuration
    }
}
