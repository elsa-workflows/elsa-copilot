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
│   ├── Program.cs                    # Application entry point
│   ├── Setup/                        # Configuration modules
│   │   ├── ElsaServerSetup.cs       # Elsa Server DI configuration
│   │   ├── ElsaStudioSetup.cs       # Elsa Studio DI configuration
│   │   └── ModuleRegistration.cs    # Custom module registration
│   ├── Pages/                        # Blazor Server pages
│   └── appsettings.json             # Application configuration
└── Modules/
    ├── Core/                         # Elsa Server modules
    │   └── Elsa.Copilot.Modules.Core.Placeholder/
    └── Studio/                       # Elsa Studio modules
        └── Elsa.Copilot.Modules.Studio.Placeholder/
```

---

## Development Workflow

### Building

```bash
# Build the entire solution
dotnet build

# Build the workbench only
dotnet build src/Elsa.Copilot.Workbench
```

### Running

```bash
# Run with default settings (uses Development environment)
cd src/Elsa.Copilot.Workbench
dotnet run

# Run with specific environment
dotnet run --environment Production

# Run with custom URLs
dotnet run --urls "http://localhost:5000;https://localhost:5001"
```

The application will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

### Debugging

#### Visual Studio
1. Open `Elsa.Copilot.Workbench.sln`
2. Set `Elsa.Copilot.Workbench` as the startup project
3. Press F5 to start debugging

#### VS Code
1. Open the workspace root
2. Use the provided launch configuration (`.vscode/launch.json`)
3. Press F5 to start debugging

#### Rider
1. Open `Elsa.Copilot.Workbench.sln`
2. Select the `Elsa.Copilot.Workbench` run configuration
3. Click the Debug button or press Shift+F9

### Database

The application uses SQLite for persistence by default:
- **Development**: `copilot-dev.db` (configured in `appsettings.Development.json`)
- **Production**: `copilot.db` (configured in `appsettings.json`)

Database migrations are applied automatically on startup.

To reset the database:
```bash
rm src/Elsa.Copilot.Workbench/copilot*.db*
dotnet run
```

---

## Adding Copilot Modules

### Overview

Elsa Copilot uses a modular architecture where Copilot features are delivered as optional modules that integrate with Elsa Server and Elsa Studio.

### Module Types

1. **Core Modules** (`src/Modules/Core/`)
   - Server-side logic (activities, services, API endpoints)
   - GitHub Copilot SDK integration
   - Function calling implementations
   - Placed in `src/Modules/Core/Elsa.Copilot.Modules.{FeatureName}/`

2. **Studio Modules** (`src/Modules/Studio/`)
   - UI components (Blazor components)
   - Chat sidebar and interface elements
   - Proposal review UI
   - Placed in `src/Modules/Studio/Elsa.Copilot.Modules.{FeatureName}/`

### Creating a New Module

#### 1. Create the Module Project

**For a Core (Server) module:**
```bash
cd src/Modules/Core
dotnet new classlib -n Elsa.Copilot.Modules.MyFeature
cd Elsa.Copilot.Modules.MyFeature
dotnet add package Elsa
```

**For a Studio module:**
```bash
cd src/Modules/Studio
dotnet new razorclasslib -n Elsa.Copilot.Modules.Studio.MyFeature
cd Elsa.Copilot.Modules.Studio.MyFeature
dotnet add package Elsa.Studio
dotnet add package Elsa.Studio.Core
```

#### 2. Add Module to Solution

```bash
cd /path/to/elsa-copilot
dotnet sln add src/Modules/Core/Elsa.Copilot.Modules.MyFeature
# or
dotnet sln add src/Modules/Studio/Elsa.Copilot.Modules.Studio.MyFeature
```

#### 3. Reference in Workbench

Edit `src/Elsa.Copilot.Workbench/Elsa.Copilot.Workbench.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="..\Modules\Core\Elsa.Copilot.Modules.MyFeature\..." />
  <!-- or -->
  <ProjectReference Include="..\Modules\Studio\Elsa.Copilot.Modules.Studio.MyFeature\..." />
</ItemGroup>
```

#### 4. Register in ModuleRegistration.cs

Edit `src/Elsa.Copilot.Workbench/Setup/ModuleRegistration.cs`:

```csharp
internal static void RegisterModules(IServiceCollection svc)
{
    // Register your module's services
    svc.AddMyFeatureModule(); // Core module
    // or
    svc.AddMyStudioFeature(); // Studio module
}
```

### Module Implementation Guidelines

#### Core Modules
- Use Elsa's `IModule` interface for proper feature registration
- Follow Elsa's activity and service patterns
- Respect existing authorization and tenancy
- Use GitHub Copilot SDK directly (no abstraction layers)

#### Studio Modules
- Create Blazor components that integrate with Elsa Studio's shell
- Use Elsa Studio's design system and UI patterns
- Communicate with Core modules via Elsa Server's HTTP API
- Keep UI logic separate from business logic

### Example Module Structure

```text
src/Modules/Core/Elsa.Copilot.Modules.Chat/
├── Activities/                  # Custom activities
├── Services/                    # Business logic services
├── Endpoints/                   # API endpoints
└── Features.cs                  # Feature registration

src/Modules/Studio/Elsa.Copilot.Modules.Studio.Chat/
├── Components/                  # Blazor components
├── Services/                    # Client-side services
└── Features.cs                  # Studio feature registration
```

---

## Requirements & Roadmap

- **Functional Requirements**: See [functional-requirements.md](functional-requirements.md)
- **Development Guidelines**: See [DEVELOPMENT.md](DEVELOPMENT.md)
- **Docker Deployment**: See [DOCKER.md](DOCKER.md)
- **Configuration**: See [src/Elsa.Copilot.Workbench/CONFIGURATION.md](src/Elsa.Copilot.Workbench/CONFIGURATION.md)

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
