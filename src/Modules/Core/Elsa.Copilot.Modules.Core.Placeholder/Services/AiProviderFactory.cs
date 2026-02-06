using Elsa.Copilot.Modules.Core.Placeholder.Abstractions;
using Elsa.Copilot.Modules.Core.Placeholder.Models;
using Microsoft.Extensions.Options;

namespace Elsa.Copilot.Modules.Core.Placeholder.Services;

/// <summary>
/// Default implementation of IAiProviderFactory.
/// </summary>
public class AiProviderFactory : IAiProviderFactory
{
    private readonly IAiProviderRegistry _registry;
    private readonly IOptions<AiProviderOptions> _options;

    public AiProviderFactory(
        IAiProviderRegistry registry,
        IOptions<AiProviderOptions> options)
    {
        _registry = registry;
        _options = options;
    }

    /// <inheritdoc />
    public IAiClient CreateClient(string? providerName = null)
    {
        var name = providerName ?? _options.Value.DefaultProvider;
        var provider = _registry.GetProvider(name);

        if (provider == null)
        {
            throw new InvalidOperationException($"AI provider '{name}' not found. Available providers: {string.Join(", ", _registry.GetAllProviders().Select(p => p.Name))}");
        }

        return provider.CreateClient();
    }
}
