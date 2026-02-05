using Elsa.Copilot.Workbench;

var builder = WebApplication.CreateBuilder(args);

// Configure all Copilot Workbench services using custom extension method
builder.Services.AddCopilotWorkbench(builder.Configuration);

var app = builder.Build();

// Configure the HTTP pipeline using custom extension method
app.UseCopilotWorkbench();

app.Run();
