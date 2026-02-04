# Functional Requirements — Elsa Studio Copilot Integration

## 1. Purpose & Scope

### Purpose

Provide **AI-assisted workflow authoring, analysis, and diagnostics** inside **Elsa Studio**, powered by **GitHub Copilot**, while keeping Elsa Studio itself **AI-agnostic** and delegating all AI responsibilities to **Elsa Server**.

### Scope

* Elsa Studio acts as a **SPA UI only**
* Elsa Server hosts all AI orchestration, security, tools, and integrations
* The AI system can:

  * Answer questions about Elsa artifacts and runtime behavior
  * Propose creation and modification of workflow definitions
  * Analyze workflow instances, errors, and trends
* The system must be **extensible by third-party Elsa modules**
* The system must respect **pluggable persistence and auth**

Out of scope:

* Direct AI SDK usage in Elsa Studio
* Auto-applying destructive changes without user review
* Hard dependency on a specific AI provider (Copilot is initial, not exclusive)

---

## 2. Architectural Constraints (Non-Negotiable)

1. **Elsa Studio**

   * Must remain a pure SPA (WASM / Server / Hybrid compatible)
   * Must not integrate any AI SDKs
   * Communicates only via HTTP(S) APIs exposed by Elsa Server

2. **Elsa Server**

   * Hosts AI orchestration and provider integration
   * Enforces authorization, tenancy, and auditing
   * Exposes all AI capabilities via stable APIs

3. **Modularity**

   * AI features must be delivered as **Elsa Server modules**
   * Third-party modules must be able to:

     * contribute AI tools (“skills”)
     * contribute AI context
   * No AI feature may bypass Elsa’s existing abstractions

---

## 3. High-Level Capabilities

### User-visible capabilities

* Conversational AI chat in Elsa Studio
* Context-aware answers (workflow, instance, tenant, selection)
* AI-assisted workflow creation and editing
* Runtime diagnostics and trend analysis
* Review-and-apply workflow changes with diffs

### System capabilities

* Tool-based AI execution (no free-form DB access)
* Streaming responses
* Deterministic mutation via proposals
* Audit trail of AI actions
* Safe extensibility for modules

---

## 4. API Surface (Elsa Server)

### Required endpoints

#### Chat & agent execution

* `POST /ai/chat`

  * Streaming response
  * Stateless or thread-based
* `POST /ai/run`

  * Executes a multi-step agent task
  * Returns progress events + final result

#### Discovery

* `GET /ai/tools`

  * Returns available AI tools with metadata
* `GET /ai/capabilities`

  * High-level feature flags for UI

#### Change application

* `POST /ai/proposals/{id}/apply`

  * Applies a previously generated proposal
  * Must re-check permissions and validity

---

## 5. AI Orchestration Requirements

### AI Orchestrator

Elsa Server must implement an **AI Orchestrator** responsible for:

1. Receiving user prompts + context handles
2. Resolving server-side context
3. Invoking the AI provider
4. Executing tool calls
5. Streaming progress/results
6. Persisting audit data

### Provider abstraction

* AI providers must be swappable
* GitHub Copilot SDK is the first implementation
* Provider-specific concerns must not leak into tools or UI

---

## 6. Context Model

### Context input from Studio

Studio sends **references**, not raw data:

* WorkflowDefinitionId
* WorkflowVersionId
* WorkflowInstanceId
* Selected ActivityId / ConnectionId
* Time windows, filters

### Context resolution (Server-side)

Server must resolve:

* Workflow graph + metadata
* Installed activities + schemas
* Runtime state, errors, bookmarks
* Relevant documentation (optional)

Context providers must be modular and composable.

---

## 7. Tool / Skill System

### Definition

An **AI Tool** is a server-side capability that:

* Has a name, description, and JSON schema
* Declares permissions and danger level
* Is invoked explicitly by the AI model
* Uses Elsa abstractions, not storage directly

### Required tool categories

#### Workflow authoring

* Read workflow
* Propose workflow creation
* Propose workflow update
* Validate workflow

#### Runtime analysis

* Query workflow instances
* Inspect instance state
* Aggregate errors
* Detect trends

#### Discovery

* Activity catalog
* Module capabilities
* Documentation search (optional)

---

## 8. Mutation Safety Model (Critical)

### Mandatory rule

**AI tools must never directly mutate state.**

### Required flow

1. AI generates a **proposal**

   * Workflow diff / patch / structured edit
2. Proposal is returned to the Studio
3. User reviews and confirms
4. Studio calls apply endpoint
5. Server re-validates and applies

No exceptions.

---

## 9. Authorization & Tenancy

* All tool invocations must:

  * run in a tenant context
  * enforce existing Elsa auth rules
* Tools declare required permissions
* Server denies tool execution if unauthorized
* Studio must not infer permissions client-side

---

## 10. Audit & Observability

The system must record:

* User prompt (sanitized)
* Tool calls (name + args summary)
* Generated proposals
* Applied changes
* Actor (user/system), tenant, timestamp

Audit data must be persistence-agnostic.

---

## 11. Extensibility Requirements

### Server-side

Third-party Elsa Server modules must be able to:

* Register AI tools
* Register context providers
* Register validation hooks

### Constraints

* Tools must declare mutability
* Mutating tools must return proposals
* Tools must respect tenancy

### Studio-side

Studio must dynamically adapt UI based on:

* Tool discovery
* Danger levels
* Permission availability

---

## 12. UX Requirements (Studio)

### Chat UI

* Streaming responses
* Context attachment controls
* Tool execution visibility

### Change review

* Visual diff for workflow changes
* Explicit “Apply” action
* Clear warnings for destructive operations

---

## 13. Non-Functional Requirements

* Must work with:

  * WASM, Server, Hybrid Studio hosts
  * Any Elsa persistence provider
  * Any Elsa auth provider
* Token usage must be minimized
* Large payloads must be resolved server-side
* Failure modes must be explicit and user-visible

---

## 14. Delivery Phases (Guidance)

1. Read-only AI chat + diagnostics
2. Workflow creation proposals
3. Workflow edit proposals + diff UI
4. Runtime trend analysis
5. Third-party skill ecosystem

---

## 15. Explicit Non-Goals

* Auto-editing production workflows without review
* Embedding AI logic in Elsa Studio
* Tight coupling to GitHub Copilot internals
* Treating AI as a trusted actor

---

## 16. Success Criteria

The implementation is successful when:

* Elsa Studio can host AI features without knowing the provider
* AI can safely reason over Elsa workflows and runtime data
* Workflow changes are transparent, reviewable, and auditable
* Third-party modules can add AI capabilities without hacks

