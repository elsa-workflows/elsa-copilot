using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;

namespace Elsa.Copilot.Modules.Core.Chat.Services;

/// <summary>
/// Mock chat client for demonstration purposes.
/// Replace with actual AI provider (OpenAI, Azure OpenAI, etc.) in production.
/// </summary>
public class MockChatClient : IChatClient
{
    public ChatClientMetadata Metadata => new("mock-chat-client", providerUri: null, modelId: "mock-model");

    public async IAsyncEnumerable<StreamingChatCompletionUpdate> CompleteStreamingAsync(
        IList<ChatMessage> chatMessages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var userMessage = chatMessages.LastOrDefault(m => m.Role == ChatRole.User)?.Text ?? string.Empty;

        var response = $"[Mock Response] You asked: '{userMessage}'\n\n" +
                      "This is a demonstration response. In production, configure an actual AI provider:\n" +
                      "- Azure OpenAI: Install Microsoft.Extensions.AI.AzureAIInference\n" +
                      "- OpenAI: Install Microsoft.Extensions.AI.OpenAI\n" +
                      "- Ollama: Install OllamaSharp and implement IChatClient\n\n" +
                      "Available tools: " + (options?.Tools?.Count ?? 0);

        var words = response.Split(' ');
        foreach (var word in words)
        {
            await Task.Delay(50, cancellationToken);
            yield return new StreamingChatCompletionUpdate
            {
                Role = ChatRole.Assistant,
                Text = word + " "
            };
        }
    }

    public Task<ChatCompletion> CompleteAsync(
        IList<ChatMessage> chatMessages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var userMessage = chatMessages.LastOrDefault(m => m.Role == ChatRole.User)?.Text ?? string.Empty;
        var response = $"[Mock Response] You asked: '{userMessage}'. Configure an actual AI provider in production.";

        return Task.FromResult(new ChatCompletion(new ChatMessage(ChatRole.Assistant, response)));
    }

    public object? GetService(Type serviceType, object? serviceKey = null) => null;

    public void Dispose() { }
}
