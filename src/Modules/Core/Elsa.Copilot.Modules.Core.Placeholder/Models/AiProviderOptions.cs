namespace Elsa.Copilot.Modules.Core.Placeholder.Models;

/// <summary>
/// Configuration options for AI providers.
/// </summary>
public class AiProviderOptions
{
    /// <summary>
    /// Gets or sets the default provider to use.
    /// </summary>
    public string DefaultProvider { get; set; } = "GitHubCopilot";

    /// <summary>
    /// Gets or sets the GitHub Copilot configuration.
    /// </summary>
    public GitHubCopilotOptions GitHubCopilot { get; set; } = new();
}

/// <summary>
/// Configuration options for GitHub Copilot.
/// </summary>
public class GitHubCopilotOptions
{
    /// <summary>
    /// Gets or sets the model to use (e.g., "gpt-4.1").
    /// </summary>
    public string Model { get; set; } = "gpt-4.1";

    /// <summary>
    /// Gets or sets additional configuration options.
    /// </summary>
    public Dictionary<string, string>? AdditionalOptions { get; set; }
}
