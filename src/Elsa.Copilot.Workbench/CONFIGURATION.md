# Application Settings Configuration

This document describes the configuration structure for the Elsa Copilot Workbench application.

## Configuration Files

### appsettings.json
Base configuration file used for all environments. Contains default settings that can be overridden.

**Important Configuration Values:**

- **Elsa.Identity.SigningKey**: **REQUIRED** - Must be set via environment-specific config or environment variable. Minimum 256 bits for JWT token signing. Empty by default for security.
  - Set via: `Elsa__Identity__SigningKey` environment variable
  - Or in `appsettings.{Environment}.json`
  
- **Backend.ApiKey**: Optional API key for backend authentication. Empty by default.
  - Set via: `Backend__ApiKey` environment variable
  
- **ConnectionStrings.Sqlite**: Database connection string. Defaults to local `copilot.db` file.

### appsettings.Development.json
Local development settings with:
- Debug logging enabled
- Relaxed security settings
- 7-day token lifetime for convenience
- Pre-configured signing key (for development only)

### appsettings.Production.json
Production-ready settings with:
- Warning-level logging
- Empty signing key (must be provided via environment variable)
- Restricted CORS (empty allowed origins - configure per deployment)
- 1-day token lifetime

## Environment Variable Overrides

Override any setting using double-underscore notation:

```bash
# Signing key (REQUIRED for production)
export Elsa__Identity__SigningKey="your-secure-key-minimum-256-bits"

# Connection string
export ConnectionStrings__Sqlite="Data Source=/path/to/db.db;Cache=Shared"

# Server URLs
export Elsa__Server__BaseUrl="https://your-domain.com"
export Backend__Url="https://your-domain.com/elsa/api"

# Logging
export Logging__LogLevel__Default="Debug"

# CORS
export Cors__AllowedOrigins__0="https://your-frontend.com"
```

## Security Notes

1. **Never commit secrets** to source control
2. **Always set a strong signing key** in production (minimum 256 bits)
3. **Configure specific CORS origins** in production (don't use "*")
4. **Use environment variables** or secure configuration management for sensitive values
5. **Rotate signing keys** periodically in production

## Configuration Validation

The application will start without a signing key, but a default development key will be used. For production:
- Set `Elsa__Identity__SigningKey` environment variable
- Or configure in appsettings.Production.json (not recommended)
- Or use a secrets management system (Azure Key Vault, AWS Secrets Manager, etc.)

## Examples

### Development (local)
```bash
dotnet run --environment Development
```

### Production (Docker)
```bash
docker run -e ASPNETCORE_ENVIRONMENT=Production \
  -e Elsa__Identity__SigningKey="your-secure-key" \
  elsa-copilot-workbench
```

### Production (docker-compose)
```bash
export ELSA_SIGNING_KEY="your-secure-key"
docker-compose up -d
```
