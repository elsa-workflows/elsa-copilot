using System.Collections.Concurrent;
using Elsa.Copilot.Modules.Core.Placeholder.Abstractions;

namespace Elsa.Copilot.Modules.Core.Placeholder.Services;

/// <summary>
/// Default implementation of IAiProviderRegistry.
/// </summary>
public class AiProviderRegistry : IAiProviderRegistry
{
    private readonly ConcurrentDictionary<string, IAiProvider> _providers = new(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public void Register(IAiProvider provider)
    {
        _providers[provider.Name] = provider;
    }

    /// <inheritdoc />
    public IAiProvider? GetProvider(string name)
    {
        return _providers.TryGetValue(name, out var provider) ? provider : null;
    }

    /// <inheritdoc />
    public IEnumerable<IAiProvider> GetAllProviders()
    {
        return _providers.Values;
    }
}
