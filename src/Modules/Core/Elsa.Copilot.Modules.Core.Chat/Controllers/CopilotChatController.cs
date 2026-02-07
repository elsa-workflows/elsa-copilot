using Elsa.Copilot.Modules.Core.Chat.Models;
using Elsa.Copilot.Modules.Core.Chat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Elsa.Copilot.Modules.Core.Chat.Controllers;

/// <summary>
/// Controller for copilot chat endpoints.
/// </summary>
[ApiController]
[Route("copilot")]
[Authorize]
public class CopilotChatController : ControllerBase
{
    private readonly CopilotChatService _chatService;
    private readonly ILogger<CopilotChatController> _logger;

    public CopilotChatController(CopilotChatService chatService, ILogger<CopilotChatController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    /// <summary>
    /// Chat endpoint that streams responses using Server-Sent Events.
    /// </summary>
    /// <param name="request">The chat request with message and optional context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("chat")]
    [Produces("text/event-stream")]
    public async Task Chat([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers["Cache-Control"] = "no-cache";

        try
        {
            await foreach (var chunk in _chatService.StreamChatAsync(request, cancellationToken))
            {
                // Properly format SSE data - handle multi-line content
                var lines = chunk.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                foreach (var line in lines)
                {
                    var sseData = $"data: {line}\n";
                    var bytes = Encoding.UTF8.GetBytes(sseData);
                    await Response.Body.WriteAsync(bytes, cancellationToken);
                }
                await Response.Body.WriteAsync(Encoding.UTF8.GetBytes("\n"), cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }

            // Send completion event
            var doneMessage = "data: [DONE]\n\n";
            var doneBytes = Encoding.UTF8.GetBytes(doneMessage);
            await Response.Body.WriteAsync(doneBytes, cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Client disconnected - this is normal
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat request");
            
            // Only send error if client hasn't disconnected
            if (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                try
                {
                    var errorResponse = new { error = "An error occurred processing your request" };
                    var errorJson = JsonSerializer.Serialize(errorResponse);
                    var errorMessage = $"data: {errorJson}\n\n";
                    var errorBytes = Encoding.UTF8.GetBytes(errorMessage);
                    await Response.Body.WriteAsync(errorBytes, default);
                    await Response.Body.FlushAsync(default);
                }
                catch
                {
                    // Ignore errors when sending error message
                }
            }
        }
    }
}
