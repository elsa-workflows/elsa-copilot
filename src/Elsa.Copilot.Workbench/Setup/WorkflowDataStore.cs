using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;

namespace Elsa.Copilot.Workbench.Setup;

/// <summary>
/// Handles workflow persistence setup for the Copilot development workbench.
/// Configures both workflow definitions storage and runtime execution state.
/// </summary>
internal static class WorkflowDataStore
{
    internal static void ConfigurePersistence(IServiceCollection svc, IConfiguration cfg)
    {
        var sqliteConn = cfg.GetConnectionString("Sqlite") ?? "Data Source=copilot.db;Cache=Shared";
        
        svc.AddElsa(elsaCfg =>
        {
            // Workflow definitions management database
            elsaCfg.UseWorkflowManagement(mgmt => 
                mgmt.UseEntityFrameworkCore(db => db.UseSqlite(sqliteConn)));
            
            // Workflow execution state database
            elsaCfg.UseWorkflowRuntime(rt => 
                rt.UseEntityFrameworkCore(db => db.UseSqlite(sqliteConn)));
        });
    }
}
