namespace Elsa.Copilot.Modules.Core.Placeholder.Models;

/// <summary>
/// Represents a response from an AI provider.
/// </summary>
public class AiResponse
{
    /// <summary>
    /// Gets or sets the generated message.
    /// </summary>
    public AiMessage Message { get; set; } = new();

    /// <summary>
    /// Gets or sets the model used to generate the response.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Gets or sets usage statistics for the request.
    /// </summary>
    public AiUsageInfo? Usage { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for this response.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets optional metadata associated with the response.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Represents usage information for an AI request.
/// </summary>
public class AiUsageInfo
{
    /// <summary>
    /// Gets or sets the number of prompt tokens used.
    /// </summary>
    public int PromptTokens { get; set; }

    /// <summary>
    /// Gets or sets the number of completion tokens generated.
    /// </summary>
    public int CompletionTokens { get; set; }

    /// <summary>
    /// Gets or sets the total number of tokens used.
    /// </summary>
    public int TotalTokens { get; set; }
}
