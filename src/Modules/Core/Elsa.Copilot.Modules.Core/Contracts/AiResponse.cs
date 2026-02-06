namespace Elsa.Copilot.Modules.Core.Contracts;

/// <summary>
/// Represents a response from an AI provider.
/// </summary>
public class AiResponse
{
    /// <summary>
    /// The generated message from the AI.
    /// </summary>
    public AiMessage Message { get; set; } = new();

    /// <summary>
    /// The reason the generation stopped (e.g., "stop", "length", "content_filter").
    /// </summary>
    public string? FinishReason { get; set; }

    /// <summary>
    /// Usage statistics for the request.
    /// </summary>
    public AiUsage? Usage { get; set; }

    /// <summary>
    /// Optional metadata associated with the response.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Represents token usage statistics.
/// </summary>
public class AiUsage
{
    /// <summary>
    /// Number of tokens in the prompt.
    /// </summary>
    public int PromptTokens { get; set; }

    /// <summary>
    /// Number of tokens in the completion.
    /// </summary>
    public int CompletionTokens { get; set; }

    /// <summary>
    /// Total number of tokens used.
    /// </summary>
    public int TotalTokens { get; set; }
}
