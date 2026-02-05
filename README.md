# Elsa Copilot

**Elsa Copilot** is an AI-powered assistant for [Elsa Workflows](https://github.com/elsa-workflows/elsa-core), providing intelligent workflow generation, completion, and optimization capabilities.

## Overview

The Elsa Copilot Workbench is a hybrid ASP.NET Core application that combines:
- **Elsa Server**: The workflow engine for executing and managing workflows
- **Elsa Studio**: A Blazor Server UI for workflow design and management
- **AI Integration**: Intelligent assistance powered by various AI providers

## Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- A compatible AI provider (OpenAI, Azure OpenAI, etc.)

### Building the Solution
```bash
dotnet build Elsa.Copilot.Workbench.sln
```

### Running the Workbench
```bash
cd src/Elsa.Copilot.Workbench
dotnet run
```

## Development

For detailed development workflows, including the **Sibling Workspace Strategy** for working with `elsa-core` and `elsa-studio`, see [DEVELOPMENT.md](DEVELOPMENT.md).

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
