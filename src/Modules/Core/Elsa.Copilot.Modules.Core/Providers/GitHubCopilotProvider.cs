using Elsa.Copilot.Modules.Core.Configuration;
using Elsa.Copilot.Modules.Core.Contracts;
using Microsoft.Extensions.Options;

namespace Elsa.Copilot.Modules.Core.Providers;

/// <summary>
/// GitHub Copilot AI provider.
/// </summary>
public class GitHubCopilotProvider : IAiProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<GitHubCopilotOptions> _options;

    public GitHubCopilotProvider(IHttpClientFactory httpClientFactory, IOptions<GitHubCopilotOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
    }

    /// <inheritdoc />
    public string Name => "github-copilot";

    /// <inheritdoc />
    public string DisplayName => "GitHub Copilot";

    /// <inheritdoc />
    public IAiClient CreateClient()
    {
        var httpClient = _httpClientFactory.CreateClient(Name);
        return new GitHubCopilotClient(httpClient, _options);
    }
}
