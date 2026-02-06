# Multi-stage Dockerfile for Elsa Copilot Workbench
# Targets .NET 8.0 and optimized for production deployment

# Stage 1: Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first for better layer caching
COPY ["Elsa.Copilot.Workbench.sln", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["src/Elsa.Copilot.Workbench/Elsa.Copilot.Workbench.csproj", "src/Elsa.Copilot.Workbench/"]
COPY ["src/Modules/Core/Elsa.Copilot.Modules.Core.Placeholder/Elsa.Copilot.Modules.Core.Placeholder.csproj", "src/Modules/Core/Elsa.Copilot.Modules.Core.Placeholder/"]
COPY ["src/Modules/Studio/Elsa.Copilot.Modules.Studio.Placeholder/Elsa.Copilot.Modules.Studio.Placeholder.csproj", "src/Modules/Studio/Elsa.Copilot.Modules.Studio.Placeholder/"]

# Restore dependencies
RUN dotnet restore "Elsa.Copilot.Workbench.sln"

# Copy remaining source code
COPY . .

# Build the application
WORKDIR "/src/src/Elsa.Copilot.Workbench"
RUN dotnet build "Elsa.Copilot.Workbench.csproj" -c Release -o /app/build

# Stage 2: Publish Stage
FROM build AS publish
RUN dotnet publish "Elsa.Copilot.Workbench.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create a non-root user for running the application
RUN groupadd -r elsaapp && useradd -r -g elsaapp elsaapp

# Copy published application
COPY --from=publish /app/publish .

# Create directory for SQLite database with proper permissions
RUN mkdir -p /app/data && chown -R elsaapp:elsaapp /app/data

# Set environment variables for production
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_USE_POLLING_FILE_WATCHER=true

# Switch to non-root user
USER elsaapp

# Expose port
EXPOSE 8080

# Health check endpoint
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Set the entrypoint
ENTRYPOINT ["dotnet", "Elsa.Copilot.Workbench.dll"]
