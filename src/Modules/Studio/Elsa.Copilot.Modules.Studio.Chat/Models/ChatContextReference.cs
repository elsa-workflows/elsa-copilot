namespace Elsa.Copilot.Modules.Studio.Chat.Models;

/// <summary>
/// Represents a context reference that can be attached to the chat conversation.
/// </summary>
public class ChatContextReference
{
    /// <summary>
    /// Optional workflow definition ID for context.
    /// </summary>
    public string? WorkflowDefinitionId { get; set; }
    
    /// <summary>
    /// Optional workflow instance ID for context.
    /// </summary>
    public string? WorkflowInstanceId { get; set; }
    
    /// <summary>
    /// Optional selected activity ID for context.
    /// </summary>
    public string? SelectedActivityId { get; set; }
    
    /// <summary>
    /// Display name for the context reference.
    /// </summary>
    public string? DisplayName { get; set; }
}
