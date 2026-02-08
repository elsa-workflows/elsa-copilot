# Elsa Copilot Chat Module

A chat-based AI assistant module for Elsa Workflows that provides context-aware help and debugging capabilities.

## Features

- **Chat API Endpoint**: `POST /copilot/chat` with Server-Sent Events streaming
- **Contextual Workflow Awareness (Phase 2)**: Automatically resolves and injects workflow, instance, and activity data into AI context
- **Tool Functions**: 4 built-in tools for accessing Elsa workflow data
- **Elsa Authentication**: Uses `[Authorize]` attribute for security
- **Microsoft.Extensions.AI**: Uses the standard .NET AI abstractions

## Phase 2: Contextual Workflow Awareness

As of Phase 2, the chat endpoint now automatically resolves context references to full workflow data:

### What Gets Resolved and Injected

1. **WorkflowDefinitionId** → Full workflow definition data
   - Workflow metadata (name, description, version, published status)
   - Complete workflow structure (activities, connections, serialized definition)
   - Timestamps (created, updated)
   - Why: Gives AI complete understanding of workflow structure and design

2. **WorkflowInstanceId** → Current execution state
   - Instance status (Running, Finished, Faulted, etc.)
   - Bookmarks (activities currently waiting for input)
   - Incidents (errors and exceptions with full stack traces)
   - Workflow properties (variables and state)
   - Timestamps (created, updated, finished)
   - Why: Gives AI runtime context for debugging and diagnostics

3. **SelectedActivityId** → Activity type information
   - Activity reference in context
   - Can query activity catalog for full schema
   - Why: Gives AI context about the specific activity user is focused on

### Context Injection Strategy

- **No custom abstractions**: Uses Elsa's built-in `IWorkflowDefinitionStore` and `IWorkflowInstanceStore` directly
- **No token limiting**: Injects all relevant data without pruning or summarization
- **Security**: Respects Elsa's authorization model via `[Authorize]` on controller; all data access uses current user's tenant/permissions
- **Format**: Injected as JSON in the system prompt for maximum AI comprehension

### Example Context Injection

When you send:
```json
{
  "message": "Why did this workflow fail?",
  "workflowInstanceId": "abc123"
}
```

The AI receives the system prompt enhanced with:
```
## Current Workflow Instance State
{
  "id": "abc123",
  "definitionId": "my-workflow",
  "status": "Faulted",
  "workflowState": {
    "incidents": [
      {
        "activityId": "activity-1",
        "message": "HTTP request failed",
        "exception": "...",
        "timestamp": "2024-01-01T12:00:00Z"
      }
    ],
    ...
  }
}

## Workflow Instance Errors
{
  "instanceId": "abc123",
  "incidents": [...],
  "totalErrors": 1
}
```

## Quick Start

### 1. Add to Your Elsa Application

In your `Program.cs` or startup code:

```csharp
using Elsa.Extensions;

// After AddElsa()
builder.Services.AddCopilotChat();

// Note: AddCopilotChat() already registers controllers.
// Ensure you map controllers in your app pipeline: app.MapControllers();
```

### 2. Configure an AI Provider

The module comes with a mock chat client for demonstration. To use a real AI provider:

```csharp
using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;  // or OpenAI, Ollama, etc.

// Remove or replace the mock client registration
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var client = new AzureOpenAIClient(
        new Uri(config["AzureOpenAI:Endpoint"]),
        new AzureKeyCredential(config["AzureOpenAI:ApiKey"]));
    
    return client.AsChatClient("gpt-4");
});
```

**Supported Providers:**
- Azure OpenAI: Install `Microsoft.Extensions.AI.AzureAIInference`
- OpenAI: Install `Microsoft.Extensions.AI.OpenAI`
- Ollama: Install `OllamaSharp` and implement `IChatClient`
- Anthropic, Google, etc.: Check Microsoft.Extensions.AI documentation

### 3. Use the Chat Endpoint

**Request:**
```bash
curl -X POST https://your-app/copilot/chat \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "message": "How do I create a workflow?",
    "workflowDefinitionId": "optional-workflow-id",
    "workflowInstanceId": "optional-instance-id",
    "selectedActivityId": "optional-activity-id"
  }'
```

**Response (Server-Sent Events):**
```
data: This is
data:  a
data:  streaming
data:  response
data: [DONE]
```

## Architecture

### Tool Functions

The module includes 4 tool functions that can be called by the AI:

1. **GetWorkflowDefinitionTool** - Retrieves workflow structure and metadata
2. **GetActivityCatalogTool** - Lists available activity types
3. **GetWorkflowInstanceStateTool** - Inspects workflow instance state
4. **GetWorkflowInstanceErrorsTool** - Gets error details for failed instances

### Service Layer

- **CopilotChatService** - Handles chat streaming and tool invocation
- **MockChatClient** - Demo client (replace with real AI provider)

### API Layer

- **CopilotChatController** - REST endpoint with SSE streaming

## Function Calling (Future Enhancement)

The current implementation uses Microsoft.Extensions.AI 9.0.1-preview which has limited function calling support. 

To enable automatic function calling:

1. Upgrade to `Microsoft.Extensions.AI.Abstractions` version 10.x or higher
2. Use `AIFunctionFactory.Create()` to register tools
3. The chat client will automatically invoke tools as needed

Example (requires v10.x):
```csharp
var options = new ChatOptions
{
    Tools = 
    [
        AIFunctionFactory.Create(_workflowDefinitionTool.GetWorkflowDefinitionAsync),
        AIFunctionFactory.Create(_activityCatalogTool.GetActivityCatalogAsync),
        // ... etc
    ]
};
```

## Authentication

The chat endpoint uses Elsa's `[Authorize]` attribute. Make sure you have authentication configured in your Elsa setup:

```csharp
elsa.UseIdentity(identity => { /* ... */ });
elsa.UseDefaultAuthentication(auth => auth.UseAdminApiKey());
```

## Testing

### Mock Client Test
The mock client is automatically registered and will respond with demo messages:

```bash
# Should return a mock response explaining how to configure real AI
curl -X POST http://localhost:5000/copilot/chat \
  -H "Content-Type: application/json" \
  -d '{"message": "Hello"}'
```

### Real Provider Test
After configuring a real AI provider, test with workflow context:

```bash
curl -X POST http://localhost:5000/copilot/chat \
  -H "Content-Type: application/json" \
  -d '{
    "message": "What activities are available?",
    "workflowDefinitionId": "my-workflow"
  }'
```

## Project Structure

```
Elsa.Copilot.Modules.Core.Chat/
├── Controllers/
│   └── CopilotChatController.cs    # REST API endpoint
├── Services/
│   ├── CopilotChatService.cs       # Chat orchestration
│   └── MockChatClient.cs           # Demo implementation
├── Tools/
│   ├── GetWorkflowDefinitionTool.cs
│   ├── GetActivityCatalogTool.cs
│   ├── GetWorkflowInstanceStateTool.cs
│   └── GetWorkflowInstanceErrorsTool.cs
├── Models/
│   ├── ChatRequest.cs              # API request model
│   └── ChatContext.cs              # Workflow context
└── Extensions/
    └── CopilotChatExtensions.cs    # DI registration
```

## Dependencies

- **Elsa 3.5.3** - Workflow engine
- **Microsoft.Extensions.AI.Abstractions 9.0.1-preview** - AI abstractions
- **ASP.NET Core** - REST API and authentication

## License

Same as the parent Elsa Copilot project.
