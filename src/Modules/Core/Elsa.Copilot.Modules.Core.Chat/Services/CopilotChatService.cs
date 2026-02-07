using Elsa.Copilot.Modules.Core.Chat.Models;
using Elsa.Copilot.Modules.Core.Chat.Tools;
using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;

namespace Elsa.Copilot.Modules.Core.Chat.Services;

/// <summary>
/// Service for handling copilot chat interactions with AI function calling.
/// NOTE: Function calling with AIFunctionFactory requires Microsoft.Extensions.AI 10.x or higher.
/// Currently, tools are injected but not yet wired to the chat client. 
/// To enable function calling, upgrade to Microsoft.Extensions.AI 10.x and populate ChatOptions.Tools.
/// </summary>
public class CopilotChatService
{
    private readonly IChatClient _chatClient;
    // Tools are available for future function calling support
    private readonly GetWorkflowDefinitionTool _workflowDefinitionTool;
    private readonly GetActivityCatalogTool _activityCatalogTool;
    private readonly GetWorkflowInstanceStateTool _workflowInstanceStateTool;
    private readonly GetWorkflowInstanceErrorsTool _workflowInstanceErrorsTool;

    public CopilotChatService(
        IChatClient chatClient,
        GetWorkflowDefinitionTool workflowDefinitionTool,
        GetActivityCatalogTool activityCatalogTool,
        GetWorkflowInstanceStateTool workflowInstanceStateTool,
        GetWorkflowInstanceErrorsTool workflowInstanceErrorsTool)
    {
        _chatClient = chatClient;
        _workflowDefinitionTool = workflowDefinitionTool;
        _activityCatalogTool = activityCatalogTool;
        _workflowInstanceStateTool = workflowInstanceStateTool;
        _workflowInstanceErrorsTool = workflowInstanceErrorsTool;
    }

    /// <summary>
    /// Streams chat responses.
    /// NOTE: To enable function calling, upgrade to Microsoft.Extensions.AI 10.x and populate ChatOptions.Tools
    /// with AIFunctionFactory.Create() for each tool function.
    /// </summary>
    public async IAsyncEnumerable<string> StreamChatAsync(
        ChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var systemPrompt = BuildSystemPrompt(request);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, request.Message)
        };

        // Tools are not currently registered in ChatOptions
        // To enable: options.Tools = [ AIFunctionFactory.Create(...) ]
        var options = new ChatOptions();

        await foreach (var update in _chatClient.CompleteStreamingAsync(messages, options, cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                yield return update.Text;
            }
        }
    }

    private string BuildSystemPrompt(ChatRequest request)
    {
        var prompt = @"You are an AI assistant for Elsa Workflows, a powerful workflow engine.
You help users understand, debug, and work with workflows, activities, and workflow instances.

Be helpful, concise, and accurate.";

        if (!string.IsNullOrEmpty(request.WorkflowDefinitionId))
        {
            prompt += $"\n\nCurrent workflow definition context: {request.WorkflowDefinitionId}";
        }

        if (!string.IsNullOrEmpty(request.WorkflowInstanceId))
        {
            prompt += $"\nCurrent workflow instance context: {request.WorkflowInstanceId}";
        }

        if (!string.IsNullOrEmpty(request.SelectedActivityId))
        {
            prompt += $"\nSelected activity: {request.SelectedActivityId}";
        }

        return prompt;
    }
}
