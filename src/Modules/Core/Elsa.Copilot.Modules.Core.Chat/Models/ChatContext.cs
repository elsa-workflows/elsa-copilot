namespace Elsa.Copilot.Modules.Core.Chat.Models;

/// <summary>
/// Represents the context for a chat session.
/// </summary>
public class ChatContext
{
    public string? WorkflowDefinitionId { get; set; }
    public string? WorkflowInstanceId { get; set; }
    public string? SelectedActivityId { get; set; }
}
