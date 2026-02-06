namespace Elsa.Copilot.Modules.Core.Placeholder.Abstractions;

/// <summary>
/// A factory for creating AI clients.
/// </summary>
public interface IAiProviderFactory
{
    /// <summary>
    /// Creates an AI client for the specified provider.
    /// </summary>
    /// <param name="providerName">The name of the provider.</param>
    /// <returns>An AI client instance.</returns>
    IAiClient CreateClient(string? providerName = null);
}
