using Elsa.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Elsa workflow engine with basic configuration
builder.Services.AddElsa(elsa =>
{
    elsa.UseWorkflowManagement();
});

var app = builder.Build();

app.MapGet("/", () => "Elsa Copilot Workbench - Elsa Integrated");

app.Run();
