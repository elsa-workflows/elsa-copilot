namespace Elsa.Copilot.Modules.Studio.Chat.Models;

/// <summary>
/// Represents a chat message in the conversation.
/// </summary>
public class ChatMessage
{
    /// <summary>
    /// Unique identifier for the message.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// The message content.
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether this is a user message (true) or AI response (false).
    /// </summary>
    public bool IsUser { get; set; }
    
    /// <summary>
    /// Timestamp when the message was created.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Whether the message is still streaming (for AI responses).
    /// </summary>
    public bool IsStreaming { get; set; }
}
