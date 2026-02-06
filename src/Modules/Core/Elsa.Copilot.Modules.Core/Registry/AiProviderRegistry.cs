using Elsa.Copilot.Modules.Core.Contracts;

namespace Elsa.Copilot.Modules.Core.Registry;

/// <summary>
/// Default implementation of the AI provider registry.
/// </summary>
public class AiProviderRegistry : IAiProviderRegistry
{
    private readonly Dictionary<string, IAiProvider> _providers = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _syncLock = new();

    /// <inheritdoc />
    public void Register(IAiProvider provider)
    {
        lock (_syncLock)
        {
            _providers[provider.Name] = provider;
        }
    }

    /// <inheritdoc />
    public IAiProvider? GetProvider(string name)
    {
        lock (_syncLock)
        {
            return _providers.TryGetValue(name, out var provider) ? provider : null;
        }
    }

    /// <inheritdoc />
    public IEnumerable<IAiProvider> GetAllProviders()
    {
        lock (_syncLock)
        {
            return _providers.Values.ToList();
        }
    }
}
