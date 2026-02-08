using Elsa.Copilot.Modules.Studio.Chat.Models;

namespace Elsa.Copilot.Modules.Studio.Chat.Services;

/// <summary>
/// Maintains the ephemeral message history for the current chat session.
/// Messages are kept in memory and cleared when the session ends.
/// </summary>
public class ChatSessionState
{
    private readonly List<ChatMessage> _messages = new();
    
    /// <summary>
    /// Gets the list of messages in the current session.
    /// </summary>
    public IReadOnlyList<ChatMessage> Messages => _messages.AsReadOnly();
    
    /// <summary>
    /// Current context reference attached to the conversation.
    /// </summary>
    public ChatContextReference? CurrentContext { get; set; }
    
    /// <summary>
    /// Adds a new message to the session.
    /// </summary>
    public void AddMessage(ChatMessage message)
    {
        _messages.Add(message);
    }
    
    /// <summary>
    /// Updates an existing message (useful for streaming updates).
    /// </summary>
    public void UpdateMessage(string messageId, Action<ChatMessage> updateAction)
    {
        var message = _messages.FirstOrDefault(m => m.Id == messageId);
        if (message != null)
        {
            updateAction(message);
        }
    }
    
    /// <summary>
    /// Clears all messages from the session.
    /// </summary>
    public void ClearMessages()
    {
        _messages.Clear();
    }
}
