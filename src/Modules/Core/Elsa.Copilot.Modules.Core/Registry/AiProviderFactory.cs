using Elsa.Copilot.Modules.Core.Configuration;
using Elsa.Copilot.Modules.Core.Contracts;
using Microsoft.Extensions.Options;

namespace Elsa.Copilot.Modules.Core.Registry;

/// <summary>
/// Factory for creating AI provider instances based on configuration.
/// </summary>
public class AiProviderFactory : IAiProviderFactory
{
    private readonly IAiProviderRegistry _registry;
    private readonly IOptions<AiProvidersOptions> _options;

    public AiProviderFactory(IAiProviderRegistry registry, IOptions<AiProvidersOptions> options)
    {
        _registry = registry;
        _options = options;
    }

    /// <inheritdoc />
    public IAiProvider? GetDefaultProvider()
    {
        var defaultProviderName = _options.Value.DefaultProvider;
        
        if (string.IsNullOrWhiteSpace(defaultProviderName))
        {
            return null;
        }

        return GetProvider(defaultProviderName);
    }

    /// <inheritdoc />
    public IAiProvider? GetProvider(string name)
    {
        return _registry.GetProvider(name);
    }
}
