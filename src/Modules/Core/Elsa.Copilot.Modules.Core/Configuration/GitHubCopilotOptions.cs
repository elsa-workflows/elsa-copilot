namespace Elsa.Copilot.Modules.Core.Configuration;

/// <summary>
/// Configuration options for GitHub Copilot provider.
/// </summary>
public class GitHubCopilotOptions
{
    /// <summary>
    /// The GitHub Copilot API endpoint.
    /// </summary>
    public string Endpoint { get; set; } = "https://api.githubcopilot.com";

    /// <summary>
    /// The GitHub token for authentication.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// The model to use (e.g., "gpt-4", "gpt-3.5-turbo").
    /// </summary>
    public string Model { get; set; } = "gpt-4";

    /// <summary>
    /// The maximum number of tokens to generate in a response.
    /// </summary>
    public int MaxTokens { get; set; } = 4096;

    /// <summary>
    /// The temperature for response generation (0.0 - 1.0).
    /// </summary>
    public double Temperature { get; set; } = 0.7;
}
