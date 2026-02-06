namespace Elsa.Copilot.Modules.Core.Placeholder.Models;

/// <summary>
/// Represents a message in an AI conversation.
/// </summary>
public class AiMessage
{
    /// <summary>
    /// Gets or sets the role of the message sender (e.g., "user", "assistant", "system").
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content of the message.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets optional metadata associated with the message.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}
