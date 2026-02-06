namespace Elsa.Copilot.Modules.Core.Contracts;

/// <summary>
/// Registry for managing AI providers.
/// </summary>
public interface IAiProviderRegistry
{
    /// <summary>
    /// Registers an AI provider.
    /// </summary>
    /// <param name="provider">The provider to register.</param>
    void Register(IAiProvider provider);

    /// <summary>
    /// Gets a provider by name.
    /// </summary>
    /// <param name="name">The provider name.</param>
    /// <returns>The provider, or null if not found.</returns>
    IAiProvider? GetProvider(string name);

    /// <summary>
    /// Gets all registered providers.
    /// </summary>
    /// <returns>All registered providers.</returns>
    IEnumerable<IAiProvider> GetAllProviders();
}
