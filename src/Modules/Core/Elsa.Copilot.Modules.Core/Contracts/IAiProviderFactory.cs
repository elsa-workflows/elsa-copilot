namespace Elsa.Copilot.Modules.Core.Contracts;

/// <summary>
/// Factory for creating AI provider instances based on configuration.
/// </summary>
public interface IAiProviderFactory
{
    /// <summary>
    /// Gets the default AI provider based on configuration.
    /// </summary>
    /// <returns>The default AI provider, or null if none is configured.</returns>
    IAiProvider? GetDefaultProvider();

    /// <summary>
    /// Gets a specific AI provider by name.
    /// </summary>
    /// <param name="name">The provider name.</param>
    /// <returns>The AI provider, or null if not found.</returns>
    IAiProvider? GetProvider(string name);
}
