namespace Elsa.Copilot.Modules.Core.Configuration;

/// <summary>
/// Configuration for AI providers.
/// </summary>
public class AiProvidersOptions
{
    /// <summary>
    /// The section key in configuration.
    /// </summary>
    public const string SectionKey = "Elsa:Copilot:AiProviders";

    /// <summary>
    /// The name of the default provider to use.
    /// </summary>
    public string? DefaultProvider { get; set; }

    /// <summary>
    /// Configuration for individual providers.
    /// </summary>
    public Dictionary<string, ProviderOptions> Providers { get; set; } = new();
}

/// <summary>
/// Configuration for a specific AI provider.
/// </summary>
public class ProviderOptions
{
    /// <summary>
    /// Whether this provider is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// The type of provider (e.g., "github-copilot", "openai").
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Provider-specific configuration.
    /// </summary>
    public Dictionary<string, string> Configuration { get; set; } = new();
}
