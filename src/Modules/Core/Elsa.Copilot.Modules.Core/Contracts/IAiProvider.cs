namespace Elsa.Copilot.Modules.Core.Contracts;

/// <summary>
/// Represents an AI provider that can create AI clients.
/// </summary>
public interface IAiProvider
{
    /// <summary>
    /// Gets the unique name of the provider (e.g., "github-copilot", "openai", "azure-openai").
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets a human-readable display name for the provider.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Creates an AI client instance for this provider.
    /// </summary>
    /// <returns>An AI client instance.</returns>
    IAiClient CreateClient();
}
