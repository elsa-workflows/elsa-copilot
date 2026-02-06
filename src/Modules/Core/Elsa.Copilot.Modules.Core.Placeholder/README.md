# AI Provider Abstraction Layer

This document describes the AI Provider Abstraction Layer implemented in the Elsa Copilot project.

## Overview

The AI Provider Abstraction Layer provides a unified interface for interacting with different AI/LLM providers (GitHub Copilot, OpenAI, Azure OpenAI, etc.) while maintaining a clean, provider-agnostic API.

## Architecture

### Core Abstractions

- **`IAiProvider`**: Represents an AI provider (e.g., GitHub Copilot)
- **`IAiClient`**: Provides methods to send requests to an AI provider
- **`IAiProviderRegistry`**: Manages registered AI providers
- **`IAiProviderFactory`**: Creates AI clients for a specific provider

### Models

- **`AiMessage`**: Represents a message in a conversation
- **`AiRequest`**: Represents a request to an AI provider
- **`AiResponse`**: Represents a response from an AI provider

### Implementations

#### GitHub Copilot Provider

The GitHub Copilot provider is implemented using the official `GitHub.Copilot.SDK` NuGet package:

- **`GitHubCopilotProvider`**: IAiProvider implementation
- **`GitHubCopilotClient`**: IAiClient implementation that wraps the GitHub Copilot SDK

## Configuration

Add the following configuration to your `appsettings.json`:

```json
{
  "AiProviders": {
    "DefaultProvider": "GitHubCopilot",
    "GitHubCopilot": {
      "Model": "gpt-4.1"
    }
  }
}
```

## Usage

### Dependency Injection Setup

In your `Program.cs` or startup configuration:

```csharp
using Elsa.Copilot.Modules.Core.Placeholder;

builder.Services.AddAiProviders(builder.Configuration);
```

### Using the Factory

```csharp
public class MyService
{
    private readonly IAiProviderFactory _aiProviderFactory;

    public MyService(IAiProviderFactory aiProviderFactory)
    {
        _aiProviderFactory = aiProviderFactory;
    }

    public async Task<string> GetAiResponseAsync(string userMessage)
    {
        // Create a client using the default provider
        var client = _aiProviderFactory.CreateClient();

        // Create a request
        var request = new AiRequest
        {
            Messages = new List<AiMessage>
            {
                new AiMessage
                {
                    Role = "user",
                    Content = userMessage
                }
            }
        };

        // Send the request
        var response = await client.SendAsync(request);
        return response.Message.Content;
    }
}
```

### Streaming Responses

```csharp
public async Task StreamAiResponseAsync(string userMessage)
{
    var client = _aiProviderFactory.CreateClient();

    var request = new AiRequest
    {
        Messages = new List<AiMessage>
        {
            new AiMessage { Role = "user", Content = userMessage }
        },
        Stream = true
    };

    await foreach (var response in client.StreamAsync(request))
    {
        Console.Write(response.Message.Content);
    }
}
```

## Adding New Providers

To add a new AI provider (e.g., OpenAI):

1. Create a new provider implementation:

```csharp
public class OpenAiProvider : IAiProvider
{
    public string Name => "OpenAI";
    
    public IAiClient CreateClient()
    {
        return new OpenAiClient(...);
    }
}

public class OpenAiClient : IAiClient
{
    public async Task<AiResponse> SendAsync(AiRequest request, CancellationToken cancellationToken = default)
    {
        // Implementation using OpenAI SDK
    }

    public IAsyncEnumerable<AiResponse> StreamAsync(AiRequest request, CancellationToken cancellationToken = default)
    {
        // Implementation using OpenAI SDK
    }
}
```

2. Register the provider in `ServiceCollectionExtensions`:

```csharp
services.AddSingleton<IAiProvider, OpenAiProvider>();
```

3. Add configuration to `appsettings.json`:

```json
{
  "AiProviders": {
    "DefaultProvider": "OpenAI",
    "OpenAI": {
      "ApiKey": "your-api-key",
      "Model": "gpt-4"
    }
  }
}
```

## Requirements

- .NET 8.0 or later
- GitHub Copilot CLI installed and authenticated (for GitHub Copilot provider)
- GitHub.Copilot.SDK NuGet package (v0.1.22 or later)

## Notes

- The GitHub Copilot provider requires the GitHub Copilot CLI to be installed and authenticated on the system
- All AI client implementations should be thread-safe
- Proper disposal of clients is important to avoid resource leaks
