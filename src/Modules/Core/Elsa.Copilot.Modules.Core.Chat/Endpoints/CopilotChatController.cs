using Elsa.Copilot.Modules.Core.Chat.Models;
using Elsa.Copilot.Modules.Core.Chat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Elsa.Copilot.Modules.Core.Chat.Endpoints;

/// <summary>
/// API controller for Copilot chat interactions.
/// </summary>
[ApiController]
[Route("copilot")]
[Authorize] // Use Elsa's built-in authentication
public class CopilotChatController : ControllerBase
{
    private readonly CopilotChatService _chatService;
    private readonly ILogger<CopilotChatController> _logger;

    public CopilotChatController(
        CopilotChatService chatService,
        ILogger<CopilotChatController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    /// <summary>
    /// Processes a chat message and streams the response back using Server-Sent Events.
    /// </summary>
    [HttpPost("chat")]
    [Produces("text/event-stream")]
    public async Task Chat([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received chat request: {Message}", request.Message);

        // Set response headers for Server-Sent Events
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");
        Response.Headers.Append("X-Accel-Buffering", "no"); // Disable nginx buffering

        // Build context from request
        var context = new ChatContext
        {
            WorkflowDefinitionId = request.WorkflowDefinitionId,
            WorkflowInstanceId = request.WorkflowInstanceId,
            SelectedActivityId = request.SelectedActivityId
        };

        try
        {
            // Stream the response
            await foreach (var chunk in _chatService.ProcessChatMessageAsync(
                request.Message, 
                context, 
                cancellationToken))
            {
                // Send as Server-Sent Event
                await Response.WriteAsync($"data: {chunk}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }

            // Send completion marker
            await Response.WriteAsync("data: [DONE]\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat message");
            await Response.WriteAsync($"data: {{\"error\": \"{ex.Message}\"}}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }
}
