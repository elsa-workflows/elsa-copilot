namespace Elsa.Copilot.Modules.Core.Contracts;

/// <summary>
/// Represents a message in an AI conversation.
/// </summary>
public class AiMessage
{
    /// <summary>
    /// The role of the message sender (e.g., "system", "user", "assistant").
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// The content of the message.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Optional metadata associated with the message.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}
