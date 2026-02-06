# Functional Requirements — Elsa Studio Copilot Integration

## 1. Purpose & Scope

### Purpose

Provide **AI-assisted workflow authoring, analysis, and diagnostics** inside **Elsa Studio** and **Elsa Server**, powered exclusively by the **GitHub Copilot SDK**, delivered as a set of **optional Elsa modules**.

### Scope

* Elsa Studio hosts the **chat UI** (Blazor component) that connects to Elsa Server
* Elsa Server hosts the **AI backend** — a thin integration layer that uses the GitHub Copilot SDK to handle chat, tool calls, and context resolution
* The AI system can:
  * Answer questions about Elsa workflows, activities, and runtime state
  * Propose creation and modification of workflow definitions
  * Diagnose workflow instance errors
* The entire feature is delivered as **optional modules** — one for Elsa Server and one for Elsa Studio — that can be added or removed without affecting the rest of Elsa

Out of scope:

* Supporting AI providers other than GitHub Copilot SDK
* Provider abstraction layers or swappable AI backends
* Custom tool/skill registries (use the GitHub Copilot SDK's built-in function calling)
* Auto-applying destructive changes without user review

---

## 2. Architectural Constraints

1. **GitHub Copilot SDK Only**
   * The GitHub Copilot SDK is the sole AI integration — no provider abstraction layer
   * Tool/function definitions use the GitHub Copilot SDK's native function calling mechanism
   * No custom `IAiProvider`, `IAiClient`, or `IAiOrchestrator` abstractions

2. **Optional Modules**
   * The Copilot integration is delivered as two optional Elsa modules:
     * **Server module**: Registers API endpoints and Copilot SDK integration with Elsa Server
     * **Studio module**: Adds the chat UI component to Elsa Studio
   * Elsa Server and Elsa Studio work normally without these modules installed

3. **Studio ↔ Server Communication**
   * Studio sends chat messages and context references to Elsa Server via HTTP(S)
   * Elsa Server uses the GitHub Copilot SDK to process requests and stream responses back
   * Studio does not call the Copilot SDK directly

4. **Existing Elsa Abstractions**
   * Use Elsa's existing authorization, tenancy, and persistence — do not reinvent them
   * Use Elsa's existing module/feature registration pattern (`FeatureBase`, `IModule`)

---

## 3. High-Level Capabilities

### User-visible

* Chat sidebar in Elsa Studio for conversational AI assistance
* Context-aware answers about workflows, activities, and instances
* AI-suggested workflow creation and edits (returned as proposals for user review)
* Error explanation and diagnostic help for failed workflow instances

### System-level

* Streaming chat responses via Server-Sent Events or similar
* GitHub Copilot SDK function calling for structured tool execution
* Workflow mutations always go through a review step (no auto-apply)

---

## 4. API Surface (Elsa Server Module)

### Endpoints

* `POST /copilot/chat`
  * Accepts a user message and context references
  * Streams the response from GitHub Copilot SDK back to Studio
  * Uses Elsa's existing auth middleware

### Context references (sent by Studio)

Studio sends **references**, not raw data:

* WorkflowDefinitionId
* WorkflowInstanceId (optional)
* Selected ActivityId (optional)

The server module resolves these references to actual workflow data before passing context to the Copilot SDK.

---

## 5. Function Calling (Tools)

Use the GitHub Copilot SDK's native function calling. Define functions as simple C# methods registered with the SDK.

### Functions to implement

#### Read operations
* **GetWorkflowDefinition** — retrieve a workflow's structure and metadata
* **GetActivityCatalog** — list available activity types and their schemas
* **GetWorkflowInstanceState** — inspect a running/failed workflow instance
* **GetWorkflowInstanceErrors** — get error details for a failed instance

#### Write operations (proposals)
* **ProposeNewWorkflow** — generate a new workflow definition for user review
* **ProposeWorkflowEdit** — generate edits to an existing workflow for user review

All write operations return a structured proposal that the user must explicitly approve before changes are applied.

---

## 6. Mutation Safety

**AI must never directly mutate workflow state.**

Flow:
1. AI generates a **proposal** (new or modified workflow definition)
2. Proposal is displayed in Studio for user review
3. User explicitly approves or rejects
4. If approved, Studio calls a standard Elsa API to save the workflow

---

## 7. Authorization & Tenancy

* All requests go through Elsa's existing auth middleware
* The Copilot module does not introduce its own auth layer
* Tool functions respect the current user's permissions and tenant context
* No cross-tenant data leakage — functions only return data the user is authorized to see

---

## 8. UX Requirements (Studio Module)

### Chat UI
* Floating or dockable chat sidebar component
* Streaming responses displayed in real-time
* Context attachment controls (select which workflow/instance to discuss)

### Proposal Review
* Display proposed workflow changes for user review
* Clear "Apply" and "Reject" actions
* No auto-apply

---

## 9. Delivery Phases

1. **Phase 1**: Read-only chat — answer questions about workflows, activities, and instances
2. **Phase 2**: Workflow creation proposals — AI suggests new workflows for user review
3. **Phase 3**: Workflow edit proposals — AI suggests changes to existing workflows
4. **Phase 4**: Runtime diagnostics — error explanation and fix suggestions

---

## 10. Explicit Non-Goals

* Provider abstraction layers (no `IAiProvider`, no swappable backends)
* Custom tool/skill registries or discovery APIs
* Custom AI permission scopes or safety gate interceptors
* AI tenancy isolation separate from Elsa's existing tenancy
* Pluggable AI audit logging (use Elsa's existing audit/logging)
* Third-party AI skill ecosystem
* Trend analysis or aggregated metrics

---

## 11. Success Criteria

The implementation is successful when:

* A user can install the Copilot modules and get a working chat experience in Elsa Studio
* The AI can answer questions about workflows and suggest workflow changes
* Workflow changes require explicit user approval
* The feature can be added or removed without affecting Elsa's core functionality
* The codebase is simple, maintainable, and uses the GitHub Copilot SDK directly without unnecessary abstraction layers
