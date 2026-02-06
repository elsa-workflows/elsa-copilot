namespace Elsa.Copilot.Modules.Core.Contracts;

/// <summary>
/// Represents an AI client that can send requests to an AI provider.
/// </summary>
public interface IAiClient
{
    /// <summary>
    /// Sends a request to the AI provider and returns a response.
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
    /// <returns>An async enumerable of response chunks.</returns>
    IAsyncEnumerable<AiResponse> StreamAsync(AiRequest request, CancellationToken cancellationToken = default);
}
