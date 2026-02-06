# Elsa Copilot

**Elsa Copilot** is an AI-powered assistant for [Elsa Workflows](https://github.com/elsa-workflows/elsa-core), providing intelligent workflow authoring, diagnostics, and conversational assistance powered by the **GitHub Copilot SDK**.

## Overview

Elsa Copilot is delivered as a set of **optional modules** for Elsa Server and Elsa Studio:

- **Server module**: Integrates the GitHub Copilot SDK with Elsa Server, exposing a chat API endpoint with function calling for workflow operations
- **Studio module**: Adds a chat sidebar UI to Elsa Studio for conversational AI assistance

The **Elsa Copilot Workbench** in this repository is a hybrid ASP.NET Core application that runs both Elsa Server and Elsa Studio side-by-side for development and testing.

### Design Principles

- **GitHub Copilot SDK only** — no provider abstraction layers or swappable AI backends
- **Optional modules** — Elsa works normally without Copilot installed
- **Use existing Elsa abstractions** — auth, tenancy, persistence, and module registration
- **No auto-apply** — AI-suggested workflow changes require explicit user approval

---

## Quick Start

### Prerequisites

- **.NET 8.0 SDK** or later ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Docker** (optional, for containerized deployment)

### Running Locally

1. **Clone the repository**:
   ```bash
   git clone https://github.com/elsa-workflows/elsa-copilot.git
   cd elsa-copilot
   ```

2. **Build and run**:
   ```bash
   cd src/Elsa.Copilot.Workbench
   dotnet run
   ```

3. **Access the application**:
   - Navigate to `https://localhost:5001` or `http://localhost:5000`

### Running with Docker

```bash
docker build -t elsa-copilot-workbench .
docker run -p 8080:8080 \
  -e Elsa__Identity__SigningKey="your-secure-key-minimum-256-bits" \
  elsa-copilot-workbench
```

For detailed Docker configuration, see [DOCKER.md](DOCKER.md).

---

## Sibling Workspace Strategy

For development across `elsa-core`, `elsa-studio`, and `elsa-copilot` simultaneously, maintain this folder structure:

```text
/development
  ├── elsa-workflows/elsa-core
  ├── elsa-workflows/elsa-studio
  └── elsa-workflows/elsa-copilot (this repository)
```

When sibling repos are detected, `Directory.Build.props` automatically swaps NuGet references for local project references.

### Architectural Rule

- **Extension points** (interfaces, component slots, hooks) go in `elsa-core` or `elsa-studio`
- **Implementation** (Copilot SDK integration, chat logic) goes in `elsa-copilot`

For details, see [DEVELOPMENT.md](DEVELOPMENT.md).

---

## Project Structure

```text
src/
├── Elsa.Copilot.Workbench/          # Development host (Server + Studio)
└── Modules/
    ├── Core/                         # Elsa Server modules
    │   └── Elsa.Copilot.Modules.Core.Placeholder/
    └── Studio/                       # Elsa Studio modules
        └── Elsa.Copilot.Modules.Studio.Placeholder/
```

---

## Requirements & Roadmap

- **Functional Requirements**: See [functional-requirements.md](functional-requirements.md)
- **Development Guidelines**: See [DEVELOPMENT.md](DEVELOPMENT.md)
- **Docker Deployment**: See [DOCKER.md](DOCKER.md)

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
