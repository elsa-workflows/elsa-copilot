using Elsa.Copilot.Modules.Core.Placeholder.Abstractions;
using Elsa.Copilot.Modules.Core.Placeholder.Models;
using GitHub.Copilot.SDK;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Elsa.Copilot.Modules.Core.Placeholder.Providers.GitHubCopilot;

/// <summary>
/// GitHub Copilot implementation of IAiClient.
/// </summary>
public class GitHubCopilotClient : IAiClient, IAsyncDisposable
{
    private readonly ILogger<GitHubCopilotClient> _logger;
    private readonly GitHubCopilotOptions _options;
    private readonly CopilotClient _copilotClient;
    private CopilotSession? _session;

    public GitHubCopilotClient(
        ILogger<GitHubCopilotClient> logger,
        IOptions<AiProviderOptions> options)
    {
        _logger = logger;
        _options = options.Value.GitHubCopilot;
        _copilotClient = new CopilotClient();
    }

    private async Task<CopilotSession> GetOrCreateSessionAsync(CancellationToken cancellationToken = default)
    {
        if (_session == null)
        {
            var sessionConfig = new SessionConfig
            {
                Model = _options.Model
            };
            _session = await _copilotClient.CreateSessionAsync(sessionConfig, cancellationToken);
        }
        return _session;
    }

    /// <inheritdoc />
    public async Task<AiResponse> SendAsync(AiRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var session = await GetOrCreateSessionAsync(cancellationToken);

            // Build the prompt from messages
            var prompt = BuildPrompt(request.Messages);

            var messageOptions = new MessageOptions
            {
                Prompt = prompt
            };

            // Send and wait for response
            var response = await session.SendAndWaitAsync(messageOptions, timeout: null, cancellationToken);

            return new AiResponse
            {
                Message = new AiMessage
                {
                    Role = "assistant",
                    Content = response?.Data.Content ?? string.Empty
                },
                Model = _options.Model,
                Id = Guid.NewGuid().ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending request to GitHub Copilot");
            throw;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<AiResponse> StreamAsync(
        AiRequest request, 
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var session = await GetOrCreateSessionAsync(cancellationToken);
        var prompt = BuildPrompt(request.Messages);
        var messageOptions = new MessageOptions { Prompt = prompt };

        var responseContent = string.Empty;
        var isComplete = false;

        // Subscribe to events
        session.On(evt =>
        {
            if (evt is AssistantMessageDeltaEvent delta)
            {
                responseContent += delta.Data.DeltaContent;
            }
            else if (evt is SessionIdleEvent)
            {
                isComplete = true;
            }
        });

        // Send the message
        await session.SendAsync(messageOptions, cancellationToken);

        // Yield responses as they come in
        var lastYieldedLength = 0;
        while (!isComplete && !cancellationToken.IsCancellationRequested)
        {
            if (responseContent.Length > lastYieldedLength)
            {
                yield return new AiResponse
                {
                    Message = new AiMessage
                    {
                        Role = "assistant",
                        Content = responseContent
                    },
                    Model = _options.Model,
                    Id = Guid.NewGuid().ToString()
                };
                lastYieldedLength = responseContent.Length;
            }
            await Task.Delay(10, cancellationToken);
        }

        // Yield the final complete response
        if (responseContent.Length > lastYieldedLength)
        {
            yield return new AiResponse
            {
                Message = new AiMessage
                {
                    Role = "assistant",
                    Content = responseContent
                },
                Model = _options.Model,
                Id = Guid.NewGuid().ToString()
            };
        }
    }

    private static string BuildPrompt(List<AiMessage> messages)
    {
        // Convert messages to a single prompt string
        // For more complex scenarios, this could be enhanced
        var lastUserMessage = messages.LastOrDefault(m => m.Role.Equals("user", StringComparison.OrdinalIgnoreCase));
        return lastUserMessage?.Content ?? string.Empty;
    }

    public async ValueTask DisposeAsync()
    {
        if (_session != null)
        {
            await _session.DisposeAsync();
        }
        await _copilotClient.DisposeAsync();
    }
}
