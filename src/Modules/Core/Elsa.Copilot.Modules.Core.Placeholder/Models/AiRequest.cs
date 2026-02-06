namespace Elsa.Copilot.Modules.Core.Placeholder.Models;

/// <summary>
/// Represents a request to an AI provider.
/// </summary>
public class AiRequest
{
    /// <summary>
    /// Gets or sets the list of messages in the conversation.
    /// </summary>
    public List<AiMessage> Messages { get; set; } = new();

    /// <summary>
    /// Gets or sets the model name to use for the request.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Gets or sets the temperature for response generation (0.0 to 1.0).
    /// </summary>
    public double? Temperature { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of tokens to generate.
    /// </summary>
    public int? MaxTokens { get; set; }

    /// <summary>
    /// Gets or sets whether to stream the response.
    /// </summary>
    public bool Stream { get; set; }

    /// <summary>
    /// Gets or sets optional additional parameters.
    /// </summary>
    public Dictionary<string, object>? AdditionalParameters { get; set; }
}
