namespace Elsa.Copilot.Workbench;

/// <summary>
/// Extension methods for configuring the Copilot Workbench HTTP pipeline.
/// This sets up the middleware ordering for the hybrid server + studio application.
/// </summary>
public static class CopilotWorkbenchApplicationExtensions
{
    /// <summary>
    /// Configures the HTTP request pipeline for the Copilot Workbench.
    /// Order matters: static files before auth, CORS before static files, etc.
    /// </summary>
    public static WebApplication UseCopilotWorkbench(this WebApplication app)
    {
        // Development vs Production configuration
        if (app.Environment.IsDevelopment())
        {
            ConfigureDevelopmentPipeline(app);
        }
        else
        {
            ConfigureProductionPipeline(app);
        }
        
        // Core middleware that's needed in all environments
        ConfigureCorePipeline(app);
        
        return app;
    }
    
    private static void ConfigureDevelopmentPipeline(WebApplication app)
    {
        // Development-specific middleware
        app.UseDeveloperExceptionPage();
    }
    
    private static void ConfigureProductionPipeline(WebApplication app)
    {
        // Production error handling
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }
    
    private static void ConfigureCorePipeline(WebApplication app)
    {
        // Core middleware stack
        app.UseHttpsRedirection();
        app.UseRouting();
        
        // Placeholders for additional middleware
        // Will be filled in with actual Elsa/Studio middleware
    }
}
