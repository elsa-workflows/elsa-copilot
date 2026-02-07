namespace Elsa.Copilot.Modules.Core.Chat.Models;

/// <summary>
/// Request model for the chat endpoint.
/// </summary>
public class ChatRequest
{
    /// <summary>
    /// The user's message.
    /// </summary>
    public required string Message { get; set; }
    
    /// <summary>
    /// Optional context reference to a workflow definition.
    /// </summary>
    public string? WorkflowDefinitionId { get; set; }
    
    /// <summary>
    /// Optional context reference to a workflow instance.
    /// </summary>
    public string? WorkflowInstanceId { get; set; }
    
    /// <summary>
    /// Optional context reference to a selected activity.
    /// </summary>
    public string? SelectedActivityId { get; set; }
}
