namespace Elsa.Copilot.Modules.Core.Contracts;

/// <summary>
/// Represents a request to an AI provider.
/// </summary>
public class AiRequest
{
    /// <summary>
    /// The messages in the conversation history.
    /// </summary>
    public IList<AiMessage> Messages { get; set; } = new List<AiMessage>();

    /// <summary>
    /// Optional temperature setting for response generation (0.0 - 1.0).
    /// </summary>
    public double? Temperature { get; set; }

    /// <summary>
    /// Optional maximum number of tokens to generate.
    /// </summary>
    public int? MaxTokens { get; set; }

    /// <summary>
    /// Whether to stream the response.
    /// </summary>
    public bool Stream { get; set; }

    /// <summary>
    /// Optional metadata associated with the request.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}
