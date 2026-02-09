using Elsa.Copilot.Modules.Core.Chat.Models;
using Elsa.Copilot.Modules.Core.Chat.Tools;
using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Elsa.Copilot.Modules.Core.Chat.Services;

/// <summary>
/// Service for handling copilot chat interactions with contextual workflow awareness.
/// 
/// Phase 2: Contextual Workflow Awareness
/// This service resolves WorkflowDefinitionId/InstanceId references to full workflow data
/// and injects relevant state, logs, and context into the AI system prompt.
/// 
/// Context Injection Strategy:
/// - WorkflowDefinitionId: Resolves to workflow structure, metadata, and activities
/// - WorkflowInstanceId: Resolves to current state, bookmarks, incidents, and properties
/// - SelectedActivityId: Adds activity ID reference to context (full details available via tools when function calling is enabled)
/// 
/// Security: Data access respects Elsa's built-in authorization via [Authorize] attribute
/// on the controller. All store access uses the current user's context automatically.
/// 
/// WARNING: Workflow instance errors may contain sensitive data (connection strings, tokens, etc.)
/// in exception stack traces. This data is sent to the configured AI provider.
/// 
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
    private readonly GetWorkflowDiagnosticsSnapshotTool _workflowDiagnosticsSnapshotTool;

    public CopilotChatService(
        IChatClient chatClient,
        GetWorkflowDefinitionTool workflowDefinitionTool,
        GetActivityCatalogTool activityCatalogTool,
        GetWorkflowInstanceStateTool workflowInstanceStateTool,
        GetWorkflowInstanceErrorsTool workflowInstanceErrorsTool,
        GetWorkflowDiagnosticsSnapshotTool workflowDiagnosticsSnapshotTool)
    {
        _chatClient = chatClient;
        _workflowDefinitionTool = workflowDefinitionTool;
        _activityCatalogTool = activityCatalogTool;
        _workflowInstanceStateTool = workflowInstanceStateTool;
        _workflowInstanceErrorsTool = workflowInstanceErrorsTool;
        _workflowDiagnosticsSnapshotTool = workflowDiagnosticsSnapshotTool;
    }

    /// <summary>
    /// Streams chat responses with contextual workflow awareness.
    /// 
    /// Phase 2 Enhancement: Resolves WorkflowDefinitionId/InstanceId/ActivityId references
    /// to full data and injects into AI context before streaming the response.
    /// 
    /// NOTE: To enable function calling, upgrade to Microsoft.Extensions.AI 10.x and populate ChatOptions.Tools
    /// with AIFunctionFactory.Create() for each tool function.
    /// </summary>
    public async IAsyncEnumerable<string> StreamChatAsync(
        ChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var systemPrompt = await BuildSystemPromptWithContextAsync(request, cancellationToken);

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

    /// <summary>
    /// Builds a system prompt with resolved workflow context data.
    /// 
    /// Context Resolution Strategy (Phase 2):
    /// 
    /// 1. WorkflowDefinitionId: Resolves to workflow metadata and structure
    ///    - Includes: name, description, version, activities, and workflow definition data
    ///    - Why: Gives AI understanding of what workflow the user is working with
    /// 
    /// 2. WorkflowInstanceId: Resolves to current execution state
    ///    - Includes: status, bookmarks (waiting activities), incidents count, properties count
    ///    - Also includes detailed error information if incidents exist (WARNING: may contain sensitive data)
    ///    - Why: Gives AI context about runtime state and issues
    /// 
    /// 3. SelectedActivityId: Adds activity ID reference
    ///    - Includes: activity ID only (not full type information)
    ///    - Why: Lets AI know which activity the user is focused on
    /// 
    /// No custom abstractions: Uses Elsa's built-in stores directly.
    /// No token limiting: Injects all relevant data without pruning.
    /// Security: Respects user permissions via Elsa's authorization model.
    /// </summary>
    private async Task<string> BuildSystemPromptWithContextAsync(
        ChatRequest request, 
        CancellationToken cancellationToken)
    {
        var prompt = @"You are an AI assistant for Elsa Workflows, a powerful workflow engine.
You help users understand, debug, and work with workflows, activities, and workflow instances.

Be helpful, concise, and accurate.

## Diagnostic & Troubleshooting Capabilities (Phase 4)
When analyzing workflow failures:
1. Identify the root cause by examining the execution timeline and incidents
2. Consider the sequence of activities that executed before the failure
3. Look for patterns like timeout errors, connection issues, or null reference exceptions
4. Provide clear explanations of what went wrong and why
5. Suggest specific, actionable next steps to resolve the issue
6. Reference specific activity IDs and types when explaining problems

Use the GetWorkflowDiagnosticsSnapshot tool when you need comprehensive failure analysis.";

        // Phase 2: Resolve and inject workflow definition context
        if (!string.IsNullOrEmpty(request.WorkflowDefinitionId))
        {
            var workflowData = await _workflowDefinitionTool.GetWorkflowDefinitionAsync(
                request.WorkflowDefinitionId, 
                cancellationToken);
            
            prompt += "\n\n## Current Workflow Definition Context\n";
            prompt += JsonSerializer.Serialize(workflowData, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
        }

        // Phase 2: Resolve and inject workflow instance context
        if (!string.IsNullOrEmpty(request.WorkflowInstanceId))
        {
            var instanceState = await _workflowInstanceStateTool.GetWorkflowInstanceStateAsync(
                request.WorkflowInstanceId, 
                cancellationToken);
            
            prompt += "\n\n## Current Workflow Instance State\n";
            prompt += JsonSerializer.Serialize(instanceState, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });

            // Also include error details if the instance has any incidents
            // Phase 4: Include comprehensive diagnostic snapshot for failed instances
            var stateJson = JsonSerializer.Serialize(instanceState);
            using (var doc = JsonDocument.Parse(stateJson))
            {
                var root = doc.RootElement;
                if (root.TryGetProperty("workflowState", out var workflowStateElement) &&
                    workflowStateElement.TryGetProperty("incidents", out var incidentsElement) &&
                    incidentsElement.ValueKind == JsonValueKind.Number &&
                    incidentsElement.TryGetInt32(out var incidentsCount) &&
                    incidentsCount > 0)
                {
                    var errors = await _workflowInstanceErrorsTool.GetWorkflowInstanceErrorsAsync(
                        request.WorkflowInstanceId, 
                        cancellationToken);

                    prompt += "\n\n## Workflow Instance Errors\n";
                    prompt += JsonSerializer.Serialize(errors, new JsonSerializerOptions 
                    { 
                        WriteIndented = true 
                    });

                    // Phase 4: Include comprehensive diagnostic snapshot for better AI analysis
                    try
                    {
                        var diagnosticSnapshot = await _workflowDiagnosticsSnapshotTool.GetDiagnosticsSnapshotAsync(
                            request.WorkflowInstanceId,
                            cancellationToken);

                        prompt += "\n\n## Comprehensive Diagnostic Snapshot\n";
                        prompt += JsonSerializer.Serialize(diagnosticSnapshot, new JsonSerializerOptions 
                        { 
                            WriteIndented = true 
                        });
                    }
                    catch (Exception)
                    {
                        // If diagnostic snapshot fails, continue with basic error information
                        // The errors above are still available for analysis
                    }
                }
            }
        }

        // Phase 2: Resolve and inject selected activity context
        // Note: Only adding a reference to the activity ID here.
        if (!string.IsNullOrEmpty(request.SelectedActivityId))
        {
            prompt += "\n\n## Selected Activity Context\n";
            prompt += $"Activity ID: {request.SelectedActivityId}\n";
        }

        return prompt;
    }
}
