using Elsa.Copilot.Modules.Core.Chat.Models;
using Elsa.Copilot.Modules.Core.Chat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    public CopilotChatController(CopilotChatService chatService)
    {
        _chatService = chatService;
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
                var sseData = $"data: {chunk}\n\n";
                var bytes = Encoding.UTF8.GetBytes(sseData);
                await Response.Body.WriteAsync(bytes, cancellationToken);
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
            var errorResponse = new { error = ex.Message };
            var errorJson = JsonSerializer.Serialize(errorResponse);
            var errorMessage = $"data: {errorJson}\n\n";
            var errorBytes = Encoding.UTF8.GetBytes(errorMessage);
            await Response.Body.WriteAsync(errorBytes, cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }
}
