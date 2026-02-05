# Elsa Copilot Development Workflow

This repository uses a **Sibling Workspace Strategy** for development. This allows for simultaneous development across `elsa-core`, `elsa-studio`, and `elsa-copilot` without the complexity of Git submodules.

## Required Directory Structure
To enable local project referencing, maintain the following folder structure:
```text
/development
  ├── elsa-workflows/elsa-core
  ├── elsa-workflows/elsa-studio
  └── elsa-workflows/elsa-copilot (This repo)
```

## Local Development Override
The `Directory.Build.props` file in this repository is configured to automatically swap NuGet package references for local Project References if the sibling directories are detected.

## Architectural Rule of Thumb
- **Extension Points:** Interfaces, Blazor component slots, and core hooks must be PR'd to `elsa-core` or `elsa-studio`.
- **Implementation (The Brains):** AI provider logic, Copilot-specific features, and the Workbench host belong in this repo.

## Pull Request Policy
1. Changes to `elsa-core` (Extension Points).
2. Changes to `elsa-studio` (UI Hooks).
3. Changes to `elsa-copilot` (Implementation).
