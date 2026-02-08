using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components;

namespace Elsa.Copilot.Modules.Studio.Chat.Services;

/// <summary>
/// Client service for communicating with the Copilot Chat API endpoint.
/// Handles Server-Sent Events (SSE) streaming from the /copilot/chat endpoint.
/// </summary>
public class StudioChatClient
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;

    public StudioChatClient(IHttpClientFactory httpClientFactory, NavigationManager navigationManager)
    {
        _httpClient = httpClientFactory.CreateClient();
        _navigationManager = navigationManager;
        
        // Set base address from NavigationManager
        _httpClient.BaseAddress = new Uri(_navigationManager.BaseUri);
    }

    /// <summary>
    /// Sends a chat message and streams the response.
    /// </summary>
    /// <param name="message">The user's message.</param>
    /// <param name="workflowDefinitionId">Optional workflow definition ID for context.</param>
    /// <param name="workflowInstanceId">Optional workflow instance ID for context.</param>
    /// <param name="selectedActivityId">Optional selected activity ID for context.</param>
    /// <param name="onChunk">Callback invoked for each chunk of the streamed response.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task StreamChatAsync(
        string message,
        string? workflowDefinitionId,
        string? workflowInstanceId,
        string? selectedActivityId,
        Action<string> onChunk,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            message,
            workflowDefinitionId,
            workflowInstanceId,
            selectedActivityId
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/copilot/chat")
        {
            Content = content
        };

        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

        using var response = await _httpClient.SendAsync(
            httpRequest,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        var buffer = new StringBuilder();

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (string.IsNullOrEmpty(line))
                continue;

            // SSE format: "data: {content}"
            if (line.StartsWith("data: "))
            {
                var data = line[6..]; // Remove "data: " prefix

                // Check for completion marker
                if (data == "[DONE]")
                    break;

                // Invoke callback with the chunk
                onChunk(data);
            }
        }
    }
}
