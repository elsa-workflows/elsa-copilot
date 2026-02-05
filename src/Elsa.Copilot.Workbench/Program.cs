using Elsa.Copilot.Workbench.Setup;
using Elsa.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure workflow persistence
WorkflowDataStore.ConfigurePersistence(builder.Services, builder.Configuration);

// Configure workflow HTTP activities and API
WorkflowApiSetup.AddHttpAndApi(builder.Services, builder.Configuration);

// Configure Studio UI
StudioUiSetup.AddStudioModules(builder.Services, builder.Configuration);

var app = builder.Build();

// Development vs Production error handling
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Request pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();
// NOTE: UseWorkflowsApi() and UseWorkflows() intentionally omitted for MVP
// They require FastEndpoints configuration and cause middleware conflicts
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
