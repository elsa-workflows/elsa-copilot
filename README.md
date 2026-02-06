# Elsa Copilot

**Elsa Copilot** is an AI-powered assistant for [Elsa Workflows](https://github.com/elsa-workflows/elsa-core), providing intelligent workflow generation, completion, and optimization capabilities.

## Overview

The **Elsa Copilot Workbench** is a hybrid ASP.NET Core application that runs both **Elsa Server** and **Elsa Studio** side-by-side in a single host. This unified approach simplifies development and deployment while maintaining the architectural separation between the workflow engine and the user interface.

### Key Components

- **Elsa Server**: The workflow engine for executing and managing workflows, including all AI orchestration and tool execution
- **Elsa Studio**: A Blazor Server UI for workflow design and management
- **AI Integration**: Intelligent assistance powered by pluggable AI providers (GitHub Copilot, OpenAI, Azure OpenAI, etc.)
- **Module System**: Extensible architecture for adding both server-side (Core) and UI (Studio) modules

This workbench serves as the foundation for Phase 0 of the Elsa Copilot project, establishing the core infrastructure and development patterns.

---

## Quick Start

### Prerequisites

- **.NET 8.0 SDK** or later ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Docker** (optional, for containerized deployment)
- **Git** (for cloning and development)

### Running Locally with .NET CLI

1. **Clone the repository**:
   ```bash
   git clone https://github.com/elsa-workflows/elsa-copilot.git
   cd elsa-copilot
   ```

2. **Build the solution**:
   ```bash
   dotnet build Elsa.Copilot.Workbench.sln
   ```

3. **Run the workbench**:
   ```bash
   cd src/Elsa.Copilot.Workbench
   dotnet run
   ```

4. **Access the application**:
   - Navigate to `https://localhost:5001` or `http://localhost:5000`
   - Default credentials (Development mode): Check the console output for login details

### Running with Docker

1. **Build the Docker image**:
   ```bash
   docker build -t elsa-copilot-workbench .
   ```

2. **Run the container**:
   ```bash
   docker run -p 8080:8080 \
     -e Elsa__Identity__SigningKey="your-secure-key-minimum-256-bits" \
     -v $(pwd)/data:/app/data \
     elsa-copilot-workbench
   ```

3. **Access the application**:
   - Navigate to `http://localhost:8080`

> **Note**: For production deployments, always set a secure signing key. Generate one with:
> ```bash
> openssl rand -base64 32
> ```

### Running with Docker Compose

1. **Set the signing key** (required for production):
   ```bash
   export ELSA_SIGNING_KEY=$(openssl rand -base64 32)
   ```

2. **Start the services**:
   ```bash
   docker-compose up -d
   ```

3. **Access the application**:
   - Navigate to `http://localhost:8080`

4. **Stop the services**:
   ```bash
   docker-compose down
   ```

For detailed Docker configuration, environment variables, and deployment scenarios, see [DOCKER.md](DOCKER.md).

---

## Sibling Workspace Strategy

The Elsa Copilot project uses a **Sibling Workspace Strategy** for development across multiple repositories. This approach allows you to develop features that span `elsa-core`, `elsa-studio`, and `elsa-copilot` simultaneously without the complexity of Git submodules.

### Required Directory Structure

To enable local project referencing, maintain the following folder structure:

```text
/development (or any parent directory name)
  ├── elsa-workflows/elsa-core
  ├── elsa-workflows/elsa-studio
  └── elsa-workflows/elsa-copilot (this repository)
```

When this structure is detected, the build system automatically swaps NuGet package references for local project references via `Directory.Build.props`.

### Architectural Guidelines

- **Extension Points**: Interfaces, abstractions, Blazor component slots, and core hooks must be contributed to `elsa-core` or `elsa-studio`
- **Implementation (The Brains)**: AI provider logic, Copilot-specific features, and the Workbench host belong in `elsa-copilot`

### Development Workflow

When working on features that span multiple repositories:

1. Make changes to `elsa-core` (if adding extension points)
2. Make changes to `elsa-studio` (if adding UI hooks)
3. Implement the feature in `elsa-copilot`
4. Submit PRs in order: `elsa-core` → `elsa-studio` → `elsa-copilot`

For complete details on the architectural rules and workflow, see [DEVELOPMENT.md](DEVELOPMENT.md).

---

## Module Integration Guide

The Elsa Copilot Workbench uses an isolated module structure under `src/Modules/` that keeps server-side and UI modules separate.

### Module Structure

```text
src/
├── Elsa.Copilot.Workbench/          # The hybrid host application
└── Modules/
    ├── Core/                         # Elsa Server (Backend) Modules
    │   └── Elsa.Copilot.Modules.Core.Placeholder/
    └── Studio/                       # Elsa Studio (Frontend) Modules
        └── Elsa.Copilot.Modules.Studio.Placeholder/
```

### Adding Elsa Server (Core) Modules

Server-side modules extend Elsa Server functionality (AI tools, workflow activities, persistence providers, etc.).

1. **Create a new project** under `src/Modules/Core/`:
   ```bash
   cd src/Modules/Core
   dotnet new classlib -n Elsa.Copilot.Modules.Core.YourModule
   ```

2. **Add the Elsa Server dependencies** to your `.csproj`:
   ```xml
   <ItemGroup>
     <PackageReference Include="Elsa" Version="3.5.3" />
     <!-- Add other Elsa packages as needed -->
   </ItemGroup>
   ```

3. **Create a feature/module class** that configures services:
   ```csharp
   public class YourModuleFeature : FeatureBase
   {
       public override void Configure()
       {
           // Register services, activities, tools, etc.
       }
   }
   ```

4. **Reference the module** in `Elsa.Copilot.Workbench.csproj`:
   ```xml
   <ItemGroup>
     <ProjectReference Include="..\Modules\Core\Elsa.Copilot.Modules.Core.YourModule\Elsa.Copilot.Modules.Core.YourModule.csproj" />
   </ItemGroup>
   ```

5. **Register the module** in `Program.cs`:
   ```csharp
   builder.Services.AddElsa(elsa =>
   {
       elsa.AddFeature<YourModuleFeature>();
   });
   ```

### Adding Elsa Studio Modules

Studio modules extend the Blazor UI (new pages, components, designer extensions, etc.).

1. **Create a new Razor Class Library** under `src/Modules/Studio/`:
   ```bash
   cd src/Modules/Studio
   dotnet new razorclasslib -n Elsa.Copilot.Modules.Studio.YourModule
   ```

2. **Add the Elsa Studio dependencies** to your `.csproj`:
   ```xml
   <ItemGroup>
     <PackageReference Include="Elsa.Studio.Core" Version="3.5.3" />
     <!-- Add other Elsa Studio packages as needed -->
   </ItemGroup>
   ```

3. **Create a module class** that configures UI components:
   ```csharp
   public class YourStudioModule : IModule
   {
       public void Configure(IServiceCollection services)
       {
           // Register UI services, pages, components, etc.
       }
   }
   ```

4. **Reference the module** in `Elsa.Copilot.Workbench.csproj`:
   ```xml
   <ItemGroup>
     <ProjectReference Include="..\Modules\Studio\Elsa.Copilot.Modules.Studio.YourModule\Elsa.Copilot.Modules.Studio.YourModule.csproj" />
   </ItemGroup>
   ```

5. **Register the module** in `Program.cs`:
   ```csharp
   builder.Services.AddElsaStudio(studio =>
   {
       studio.AddModule<YourStudioModule>();
   });
   ```

---

## Debugging

### Using Visual Studio

1. **Open the solution** in Visual Studio 2022 or later
2. **Set `Elsa.Copilot.Workbench` as the startup project**
3. **Press F5** to start debugging
4. The application will launch with the debugger attached

### Using Visual Studio Code

1. **Open the workspace** in VS Code
2. **Install the C# Dev Kit extension** (if not already installed)
3. **Open the Run and Debug panel** (Ctrl+Shift+D / Cmd+Shift+D)
4. **Select ".NET Core Launch (web)"** from the dropdown
5. **Press F5** to start debugging

Alternatively, use the integrated terminal:
```bash
cd src/Elsa.Copilot.Workbench
dotnet run --launch-profile https
```

### Debugging Modules

To debug code in your custom modules:

1. **Set breakpoints** in your module source code
2. **Ensure the module project is referenced** by the Workbench
3. **Rebuild the solution** to include your changes
4. **Start debugging** from the Workbench project

The debugger will step into your module code when execution reaches your breakpoints.

### Troubleshooting

**Module not loading:**
- Verify the project reference exists in `Elsa.Copilot.Workbench.csproj`
- Check that the module is registered in `Program.cs`
- Review the console output for initialization errors

**Configuration issues:**
- Ensure `appsettings.json` is properly formatted (valid JSON)
- Check that required settings (e.g., `SigningKey`) are configured
- Use the Development environment for easier debugging: `ASPNETCORE_ENVIRONMENT=Development`

**Port conflicts:**
- The default ports are 5000 (HTTP) and 5001 (HTTPS)
- Change ports in `launchSettings.json` or via `--urls` command-line argument

For Docker-specific troubleshooting, see [DOCKER.md](DOCKER.md).

---

## Project Roadmap & Requirements

This project is being developed in phases. We are currently completing **Phase 0: Foundation**.

- **Functional Requirements**: See [functional-requirements.md](functional-requirements.md) for the complete architectural specification
- **Master Roadmap**: Track progress and upcoming features in [Issue #7: Master Roadmap](https://github.com/elsa-workflows/elsa-copilot/issues/7)
- **Development Guidelines**: See [DEVELOPMENT.md](DEVELOPMENT.md) for detailed architectural rules and workflows
- **Docker Deployment**: See [DOCKER.md](DOCKER.md) for containerization and deployment guides

---

## Contributing

Contributions are welcome! This project follows the architectural patterns established in Phase 0:

1. Review [functional-requirements.md](functional-requirements.md) to understand the system architecture
2. Check the [Master Roadmap (Issue #7)](https://github.com/elsa-workflows/elsa-copilot/issues/7) for planned features
3. Follow the [Sibling Workspace Strategy](#sibling-workspace-strategy) for multi-repo development
4. Submit PRs that align with the "extension point vs. implementation" separation

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
