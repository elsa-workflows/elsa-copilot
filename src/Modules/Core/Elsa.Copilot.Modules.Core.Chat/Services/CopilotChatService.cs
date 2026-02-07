using Elsa.Copilot.Modules.Core.Chat.Models;
using Elsa.Copilot.Modules.Core.Chat.Tools;
using GitHub.Copilot.SDK;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace Elsa.Copilot.Modules.Core.Chat.Services;

/// <summary>
/// Service for handling chat interactions using the GitHub Copilot SDK.
/// </summary>
public class CopilotChatService
{
    private readonly ILogger<CopilotChatService> _logger;
    private readonly GetWorkflowDefinitionTool _workflowDefinitionTool;
    private readonly GetActivityCatalogTool _activityCatalogTool;
    private readonly GetWorkflowInstanceStateTool _workflowInstanceStateTool;
    private readonly GetWorkflowInstanceErrorsTool _workflowInstanceErrorsTool;

    public CopilotChatService(
        ILogger<CopilotChatService> logger,
        GetWorkflowDefinitionTool workflowDefinitionTool,
        GetActivityCatalogTool activityCatalogTool,
        GetWorkflowInstanceStateTool workflowInstanceStateTool,
        GetWorkflowInstanceErrorsTool workflowInstanceErrorsTool)
    {
        _logger = logger;
        _workflowDefinitionTool = workflowDefinitionTool;
        _activityCatalogTool = activityCatalogTool;
        _workflowInstanceStateTool = workflowInstanceStateTool;
        _workflowInstanceErrorsTool = workflowInstanceErrorsTool;
    }

    /// <summary>
    /// Processes a chat message and streams the response.
    /// </summary>
    public async IAsyncEnumerable<string> ProcessChatMessageAsync(
        string message,
        ChatContext? context,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing chat message with context: WorkflowDefinitionId={WorkflowDefinitionId}, WorkflowInstanceId={WorkflowInstanceId}",
            context?.WorkflowDefinitionId, context?.WorkflowInstanceId);

        CopilotClient? client = null;
        CopilotSession? session = null;

        try
        {
            // Initialize Copilot client
            client = new CopilotClient();
            await client.StartAsync(cancellationToken);

            // Create session with tool functions
            var sessionConfig = new SessionConfig
            {
                Model = "gpt-4o" // Use GPT-4 for better reasoning
            };

            session = await client.CreateSessionAsync(sessionConfig, cancellationToken);

            // Register tool functions with the session
            await RegisterToolsAsync(session, cancellationToken);

            // Build system message with context
            var systemMessage = BuildSystemMessage(context);
            
            // Send system message first
            await session.SendAsync(new Turn
            {
                Role = "system",
                Content = systemMessage
            }, cancellationToken);

            // Send user message
            await session.SendAsync(new Turn
            {
                Role = "user",
                Content = message
            }, cancellationToken);

            // Stream the response
            await foreach (var turn in session.ReceiveAsync(cancellationToken))
            {
                if (turn.Content != null)
                {
                    yield return turn.Content;
                }

                // If the assistant is done responding, stop
                if (turn.Role == "assistant" && turn.FinishReason != null)
                {
                    break;
                }
            }
        }
        finally
        {
            if (session != null)
            {
                await session.DisposeAsync();
            }
            if (client != null)
            {
                await client.DisposeAsync();
            }
        }
    }

    private async Task RegisterToolsAsync(CopilotSession session, CancellationToken cancellationToken)
    {
        // Register GetWorkflowDefinition tool
        await session.RegisterFunctionAsync(
            "GetWorkflowDefinition",
            "Retrieves a workflow definition's structure and metadata by ID",
            async (args, ct) =>
            {
                var workflowDefinitionId = args["workflowDefinitionId"]?.ToString() ?? "";
                return await _workflowDefinitionTool.GetWorkflowDefinitionAsync(workflowDefinitionId, ct);
            },
            new Dictionary<string, object>
            {
                ["workflowDefinitionId"] = new { type = "string", description = "The workflow definition ID to retrieve" }
            },
            cancellationToken);

        // Register GetActivityCatalog tool
        await session.RegisterFunctionAsync(
            "GetActivityCatalog",
            "Lists all available activity types and their schemas",
            async (args, ct) =>
            {
                var category = args.ContainsKey("category") ? args["category"]?.ToString() : null;
                return await _activityCatalogTool.GetActivityCatalogAsync(category, ct);
            },
            new Dictionary<string, object>
            {
                ["category"] = new { type = "string", description = "Optional category filter", required = false }
            },
            cancellationToken);

        // Register GetWorkflowInstanceState tool
        await session.RegisterFunctionAsync(
            "GetWorkflowInstanceState",
            "Inspects a running or failed workflow instance's current state",
            async (args, ct) =>
            {
                var workflowInstanceId = args["workflowInstanceId"]?.ToString() ?? "";
                return await _workflowInstanceStateTool.GetWorkflowInstanceStateAsync(workflowInstanceId, ct);
            },
            new Dictionary<string, object>
            {
                ["workflowInstanceId"] = new { type = "string", description = "The workflow instance ID to inspect" }
            },
            cancellationToken);

        // Register GetWorkflowInstanceErrors tool
        await session.RegisterFunctionAsync(
            "GetWorkflowInstanceErrors",
            "Gets error details for a failed workflow instance",
            async (args, ct) =>
            {
                var workflowInstanceId = args["workflowInstanceId"]?.ToString() ?? "";
                return await _workflowInstanceErrorsTool.GetWorkflowInstanceErrorsAsync(workflowInstanceId, ct);
            },
            new Dictionary<string, object>
            {
                ["workflowInstanceId"] = new { type = "string", description = "The workflow instance ID to get errors for" }
            },
            cancellationToken);
    }

    private string BuildSystemMessage(ChatContext? context)
    {
        var message = "You are an AI assistant for Elsa Workflows, a powerful workflow automation platform. " +
                      "You help users understand, create, and troubleshoot workflows. " +
                      "You have access to tools that can retrieve workflow definitions, list available activities, " +
                      "inspect workflow instances, and get error details.";

        if (context != null)
        {
            message += "\n\nCurrent context:";
            
            if (!string.IsNullOrEmpty(context.WorkflowDefinitionId))
            {
                message += $"\n- Workflow Definition: {context.WorkflowDefinitionId}";
            }
            
            if (!string.IsNullOrEmpty(context.WorkflowInstanceId))
            {
                message += $"\n- Workflow Instance: {context.WorkflowInstanceId}";
            }
            
            if (!string.IsNullOrEmpty(context.SelectedActivityId))
            {
                message += $"\n- Selected Activity: {context.SelectedActivityId}";
            }
        }

        return message;
    }
}
