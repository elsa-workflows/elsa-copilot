using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Configure Elsa with EF Core persistence using SQLite
builder.Services.AddElsa(elsa =>
{
    // Setup workflow management with SQLite persistence  
    elsa.UseWorkflowManagement(management =>
    {
        management.UseEntityFrameworkCore(efCore =>
        {
            var connectionString = config.GetConnectionString("Sqlite") ?? "Data Source=copilot.db;Cache=Shared";
            efCore.UseSqlite(connectionString);
        });
    });
    
    // Setup workflow runtime with same database
    elsa.UseWorkflowRuntime(runtime =>
    {
        runtime.UseEntityFrameworkCore(efCore =>
        {
            var connectionString = config.GetConnectionString("Sqlite") ?? "Data Source=copilot.db;Cache=Shared";
            efCore.UseSqlite(connectionString);
        });
    });
});

var app = builder.Build();

app.MapGet("/", () => "Elsa Copilot Workbench - Persistence Configured");

app.Run();
