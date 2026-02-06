using Elsa.Copilot.Workbench.Setup;
using Elsa.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure Elsa Server (workflow engine and API)
ElsaServerSetup.AddElsaServer(builder.Services, builder.Configuration);

// Configure Elsa Studio (Blazor Server UI)
ElsaStudioSetup.AddElsaStudio(builder.Services, builder.Configuration);

// Register custom modules
ModuleRegistration.RegisterModules(builder.Services, builder.Configuration);

var app = builder.Build();

// Development vs Production error handling
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Request pipeline - order matters!
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Authentication and Authorization (even if not fully configured, middleware should be present)
app.UseAuthentication();
app.UseAuthorization();

// CORS for API access
app.UseCors();

// Elsa Server middleware - exposes workflow management and execution APIs
app.UseWorkflowsApi();
app.UseWorkflows();

// Blazor Server endpoints for Studio UI
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
