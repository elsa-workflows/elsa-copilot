using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Elsa.Copilot.Modules.Core.Configuration;
using Elsa.Copilot.Modules.Core.Contracts;
using Microsoft.Extensions.Options;

namespace Elsa.Copilot.Modules.Core.Providers;

/// <summary>
/// AI client for GitHub Copilot.
/// </summary>
public class GitHubCopilotClient : IAiClient
{
    private readonly HttpClient _httpClient;
    private readonly GitHubCopilotOptions _options;

    public GitHubCopilotClient(HttpClient httpClient, IOptions<GitHubCopilotOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    /// <inheritdoc />
    public async Task<AiResponse> SendAsync(AiRequest request, CancellationToken cancellationToken = default)
    {
        var payload = CreatePayload(request, stream: false);
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_options.Endpoint}/chat/completions")
        {
            Content = content
        };
        
        AddAuthHeaders(httpRequest);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var responseData = JsonSerializer.Deserialize<JsonElement>(responseContent);

        return ParseResponse(responseData);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<AiResponse> StreamAsync(
        AiRequest request, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var payload = CreatePayload(request, stream: true);
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_options.Endpoint}/chat/completions")
        {
            Content = content
        };
        
        AddAuthHeaders(httpRequest);

        using var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            
            if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("data: "))
            {
                continue;
            }

            var data = line[6..].Trim();
            
            if (data == "[DONE]")
            {
                break;
            }

            var chunk = JsonSerializer.Deserialize<JsonElement>(data);
            yield return ParseStreamChunk(chunk);
        }
    }

    private void AddAuthHeaders(HttpRequestMessage request)
    {
        if (!string.IsNullOrWhiteSpace(_options.Token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.Token);
        }
    }

    private object CreatePayload(AiRequest request, bool stream)
    {
        return new
        {
            model = _options.Model,
            messages = request.Messages.Select(m => new
            {
                role = m.Role,
                content = m.Content
            }),
            temperature = request.Temperature ?? _options.Temperature,
            max_tokens = request.MaxTokens ?? _options.MaxTokens,
            stream = stream
        };
    }

    private AiResponse ParseResponse(JsonElement responseData)
    {
        if (!responseData.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
        {
            throw new InvalidOperationException("The API response did not contain any choices.");
        }

        var choice = choices[0];
        var message = choice.GetProperty("message");
        var usage = responseData.TryGetProperty("usage", out var usageElement) ? usageElement : (JsonElement?)null;

        return new AiResponse
        {
            Message = new AiMessage
            {
                Role = message.GetProperty("role").GetString() ?? "assistant",
                Content = message.GetProperty("content").GetString() ?? string.Empty
            },
            FinishReason = choice.TryGetProperty("finish_reason", out var finishReason) 
                ? finishReason.GetString() 
                : null,
            Usage = usage.HasValue ? new AiUsage
            {
                PromptTokens = usage.Value.GetProperty("prompt_tokens").GetInt32(),
                CompletionTokens = usage.Value.GetProperty("completion_tokens").GetInt32(),
                TotalTokens = usage.Value.GetProperty("total_tokens").GetInt32()
            } : null
        };
    }

    private AiResponse ParseStreamChunk(JsonElement chunk)
    {
        if (!chunk.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
        {
            throw new InvalidOperationException("The API stream chunk did not contain any choices.");
        }

        var choice = choices[0];
        var delta = choice.GetProperty("delta");
        
        var content = delta.TryGetProperty("content", out var contentElement) 
            ? contentElement.GetString() ?? string.Empty 
            : string.Empty;
            
        var role = delta.TryGetProperty("role", out var roleElement) 
            ? roleElement.GetString() ?? "assistant" 
            : "assistant";

        return new AiResponse
        {
            Message = new AiMessage
            {
                Role = role,
                Content = content
            },
            FinishReason = choice.TryGetProperty("finish_reason", out var finishReason) 
                ? finishReason.GetString() 
                : null
        };
    }
}
