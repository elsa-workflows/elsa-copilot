using Elsa.Copilot.Modules.Core.Placeholder.Models;

namespace Elsa.Copilot.Modules.Core.Placeholder.Abstractions;

/// <summary>
/// Represents a client for interacting with an AI provider.
/// </summary>
public interface IAiClient
{
    /// <summary>
    /// Sends a request to the AI provider and returns the response.
    /// </summary>
    /// <param name="request">The AI request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The AI response.</returns>
    Task<AiResponse> SendAsync(AiRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request to the AI provider and streams the response.
    /// </summary>
    /// <param name="request">The AI request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of AI responses.</returns>
    IAsyncEnumerable<AiResponse> StreamAsync(AiRequest request, CancellationToken cancellationToken = default);
}
