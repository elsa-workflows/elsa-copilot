namespace Elsa.Copilot.Modules.Core.Placeholder.Abstractions;

/// <summary>
/// Represents a provider for AI services.
/// </summary>
public interface IAiProvider
{
    /// <summary>
    /// Gets the name of the provider.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Creates a new AI client instance.
    /// </summary>
    /// <returns>An AI client instance.</returns>
    IAiClient CreateClient();
}
