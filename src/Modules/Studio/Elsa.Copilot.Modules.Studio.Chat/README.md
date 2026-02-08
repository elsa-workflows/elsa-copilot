# Elsa.Copilot.Modules.Studio.Chat

Blazor UI module for the Elsa Copilot chat interface. Provides a floating/dockable chat sidebar that communicates with the `/copilot/chat` API endpoint.

## Features

- **Floating Chat Panel**: Dockable sidebar that slides in from the right
- **Real-time Streaming**: Displays AI responses as they stream from the server using Server-Sent Events (SSE)
- **Message History**: Maintains ephemeral session-based chat history
- **Context Attachment**: Attach workflow definition, instance, or activity references to the conversation
- **MudBlazor Integration**: Built using MudBlazor components for consistent Material Design UI

## Architecture

### Components

#### ChatPanel.razor
Main component that provides the chat interface:
- Floating action button (FAB) to open/close the panel
- Message display with user and AI messages
- Input field for user messages
- Context attachment dialog
- Streaming progress indicator

### Services

#### StudioChatClient
Handles communication with the `/copilot/chat` endpoint:
- Sends POST requests with chat messages and context references
- Parses Server-Sent Events (SSE) stream
- Invokes callbacks for each chunk of streamed response
- Configured with base address from NavigationManager

#### ChatSessionState
Maintains ephemeral message history for the current session:
- Stores list of ChatMessage objects
- Tracks current context reference
- Provides methods to add, update, and clear messages

### Models

#### ChatMessage
Represents a single message in the conversation:
- `Id`: Unique identifier
- `Content`: Message text
- `IsUser`: Whether message is from user or AI
- `Timestamp`: When the message was created
- `IsStreaming`: Whether the message is still being streamed

#### ChatContextReference
Represents a context reference attached to the conversation:
- `WorkflowDefinitionId`: Optional workflow definition ID
- `WorkflowInstanceId`: Optional workflow instance ID
- `SelectedActivityId`: Optional selected activity ID
- `DisplayName`: Optional display name for the reference

## Usage

### Installation

The module is automatically registered when added to the Elsa Studio setup:

```csharp
// In ElsaStudioSetup.cs
services.AddStudioChatModule();
```

### Configuration

The module requires:
- MudBlazor CSS and JS in _Host.cshtml
- MudBlazor services registered
- NavigationManager for base address configuration

### Opening the Chat Panel

1. Click the floating action button (FAB) in the bottom-right corner
2. The chat panel slides in from the right side
3. Type a message and press Enter or click Send
4. View AI responses as they stream in real-time

### Attaching Context

1. Click the "Attach Context" button in the chat footer
2. Enter Workflow Definition ID, Instance ID, or Activity ID
3. Optionally provide a display name
4. Click "Apply" to attach the context
5. The context chip appears at the top of the chat panel
6. All subsequent messages include this context

### Closing the Panel

Click the X button in the panel header or click outside the panel.

## API Endpoint

The chat panel communicates with the `/copilot/chat` endpoint:

**Request:**
```json
{
  "message": "How do I create a workflow?",
  "workflowDefinitionId": "optional-workflow-id",
  "workflowInstanceId": "optional-instance-id",
  "selectedActivityId": "optional-activity-id"
}
```

**Response:**
Server-Sent Events (SSE) stream:
```
data: I
data:  can
data:  help
data:  you
data: ...
data: [DONE]
```

## Styling

Custom CSS is defined in `ChatPanel.razor.css`:
- Floating panel positioning and animation
- Message bubble styling
- User vs AI message differentiation
- Dark mode support

## Dependencies

- **Elsa.Studio 3.5.3**: Core Elsa Studio services
- **Elsa.Studio.Core 3.5.3**: Core infrastructure
- **MudBlazor 8.11.0**: UI component library

## Security

- All chat requests go through Elsa's `[Authorize]` attribute on the controller
- HttpClient automatically includes authentication headers when configured
- Context references are resolved on the server with proper authorization checks

## Future Enhancements

- Persistent message history across sessions
- Export chat history
- Code syntax highlighting in AI responses
- Workflow proposal diff viewer
- Multi-turn conversation threading
