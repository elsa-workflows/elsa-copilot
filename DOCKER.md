# Docker Setup for Elsa Copilot Workbench

This document describes how to build and run the Elsa Copilot Workbench using Docker.

## Quick Start

### Building the Docker Image

```bash
docker build -t elsa-copilot-workbench .
```

### Running with Docker

```bash
docker run -d \
  --name elsa-copilot \
  -p 8080:8080 \
  -v $(pwd)/data:/app/data \
  -e Elsa__Identity__SigningKey="your-secret-signing-key-here-minimum-256-bits" \
  elsa-copilot-workbench
```

Access the application at: http://localhost:8080

### Running with Docker Compose

Production mode:
```bash
docker-compose up -d
```

Development mode:
```bash
docker-compose --profile dev up -d elsa-copilot-dev
```

## Configuration

### Environment-Based Configuration

The application supports three environments:
- **Development**: Uses `appsettings.Development.json` with debug logging and relaxed settings
- **Production**: Uses `appsettings.Production.json` with optimized settings for deployment
- **Custom**: Create `appsettings.{Environment}.json` for custom configurations

### Configuration Switching

Switch environments using the `ASPNETCORE_ENVIRONMENT` variable:

```bash
docker run -e ASPNETCORE_ENVIRONMENT=Development elsa-copilot-workbench
```

### Environment Variables

Override any configuration setting using environment variables with double underscore notation:

```bash
# Connection string
-e ConnectionStrings__Sqlite="Data Source=/app/data/custom.db;Cache=Shared"

# Elsa Identity signing key (REQUIRED for production)
-e Elsa__Identity__SigningKey="your-secret-key-minimum-256-bits"

# Logging level
-e Logging__LogLevel__Default="Debug"

# Server URLs
-e Elsa__Server__BaseUrl="http://your-domain.com"
-e Backend__Url="http://your-domain.com/elsa/api"

# CORS settings
-e Cors__AllowedOrigins__0="https://your-frontend.com"
```

### Volume Mounts

Mount volumes to persist data:

```bash
# SQLite database
-v $(pwd)/data:/app/data

# Custom appsettings (if needed)
-v $(pwd)/appsettings.custom.json:/app/appsettings.Production.json
```

## Development

### Local Development with Docker

For local development with hot reload, consider mounting the source code:

```bash
docker run -it --rm \
  -p 5000:8080 \
  -v $(pwd)/src:/app/src \
  -e ASPNETCORE_ENVIRONMENT=Development \
  elsa-copilot-workbench
```

### Building for Different Architectures

```bash
# AMD64
docker build --platform linux/amd64 -t elsa-copilot-workbench:amd64 .

# ARM64
docker build --platform linux/arm64 -t elsa-copilot-workbench:arm64 .
```

## Security Considerations

1. **Signing Key**: Always set a strong signing key in production:
   ```
   Elsa__Identity__SigningKey=<strong-random-key-minimum-256-bits>
   ```

2. **CORS**: Configure specific allowed origins in production:
   ```
   Cors__AllowedOrigins__0=https://your-domain.com
   ```

3. **HTTPS**: For production, use a reverse proxy (nginx, Traefik) with SSL/TLS

4. **Database**: Consider using a more robust database system for production workloads

## Troubleshooting

### Container won't start
Check logs:
```bash
docker logs elsa-copilot
```

### Database permission issues
Ensure the data directory is writable:
```bash
chmod 777 ./data
```

### Health check failing
The container includes a health check at `/health`. Verify it's accessible:
```bash
docker inspect --format='{{json .State.Health}}' elsa-copilot
```

## Multi-Stage Build Details

The Dockerfile uses a multi-stage build:
1. **Build Stage**: Restores dependencies and builds the application
2. **Publish Stage**: Publishes the application for production
3. **Runtime Stage**: Creates a minimal runtime image with only necessary files

This approach results in:
- Smaller image size (~200MB vs ~1GB)
- Better security (no build tools in production image)
- Faster deployment times

## Examples

### Production Deployment with External Database

```bash
docker run -d \
  --name elsa-copilot \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__Sqlite="Data Source=/app/data/production.db;Cache=Shared" \
  -e Elsa__Identity__SigningKey="${ELSA_SIGNING_KEY}" \
  -e Elsa__Server__BaseUrl="https://api.yourdomain.com" \
  -e Backend__Url="https://api.yourdomain.com/elsa/api" \
  -e Http__BaseUrl="https://api.yourdomain.com" \
  -e Cors__AllowedOrigins__0="https://yourdomain.com" \
  -v /var/elsa/data:/app/data \
  --restart unless-stopped \
  elsa-copilot-workbench
```

### Development with Custom Settings

```bash
docker-compose --profile dev up -d
```

Then access at: http://localhost:5000
