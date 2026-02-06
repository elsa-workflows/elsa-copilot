using Elsa.Copilot.Modules.Core.Placeholder.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa.Copilot.Modules.Core.Placeholder.Providers.GitHubCopilot;

/// <summary>
/// GitHub Copilot implementation of IAiProvider.
/// </summary>
public class GitHubCopilotProvider : IAiProvider
{
    private readonly IServiceProvider _serviceProvider;

    public GitHubCopilotProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public string Name => "GitHubCopilot";

    /// <inheritdoc />
    public IAiClient CreateClient()
    {
        return _serviceProvider.GetRequiredService<GitHubCopilotClient>();
    }
}
