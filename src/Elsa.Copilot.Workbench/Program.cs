var builder = WebApplication.CreateBuilder(args);

// TODO: Task 15 will add Elsa Server and Studio configuration here

var app = builder.Build();

app.MapGet("/", () => "Elsa Copilot Workbench");

app.Run();
