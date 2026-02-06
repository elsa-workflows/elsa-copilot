# Elsa Copilot Core Security

This module provides foundational security and tenancy guardrails for Elsa Copilot AI operations.

## Overview

The security module implements the "Skull" (security) layer required to ensure that AI operations are safe, permissioned, and tenant-isolated. It provides:

1. **Tenancy Authorization** - Enforces tenant boundaries to prevent cross-tenant data leakage
2. **AI Permission Scopes** - Granular permission system for AI-driven actions
3. **Safety Gate** - Server-side interceptor for AI tool execution with input/output validation
4. **Safety Rules** - Extensible system for implementing specific safety validations

## Components

### AI Permission Scopes

Defined in `AiPermissionScopes`, these scopes integrate with Elsa's standard authorization policy system:

- `ai:read` - Required to gather workflow/instance state for AI analysis
- `ai:propose` - Required for the AI to generate and submit change proposals
- `ai:diagnose` - Required to generate runtime failure explanations
- `ai:admin` - Required for managing AI provider and tool configurations

### Authorization Handler

`IAiAuthorizationHandler` and `DefaultAiAuthorizationHandler` enforce tenancy boundaries and validate AI permission scopes.

```csharp
// Example usage
var context = new AiAuthorizationContext
{
    TenantId = "tenant-123",
    UserId = "user-456",
    RequiredScope = AiPermissionScopes.Read
};

var result = await authHandler.AuthorizeAsync(context);
if (!result.IsAuthorized)
{
    // Handle unauthorized access
    throw new UnauthorizedAccessException(result.DenialReason);
}
```

### Safety Gate

`IAiSafetyGate` and `DefaultAiSafetyGate` intercept AI tool execution to validate inputs and outputs.

```csharp
// Example usage
var toolContext = new AiToolExecutionContext
{
    ToolName = "GetWorkflowDefinition",
    TenantId = "tenant-123",
    UserId = "user-456",
    InputParameters = new Dictionary<string, object?>
    {
        { "workflowId", "workflow-789" }
    }
};

// Validate input before execution
var inputValidation = await safetyGate.ValidateInputAsync(toolContext);
if (!inputValidation.IsValid)
{
    // Handle invalid input
    throw new InvalidOperationException(inputValidation.Message);
}

// Execute tool...
var result = await ExecuteToolAsync(toolContext);

// Validate output before returning
var outputValidation = await safetyGate.ValidateOutputAsync(toolContext, result);
if (!outputValidation.IsValid)
{
    // Handle invalid output
    throw new InvalidOperationException(outputValidation.Message);
}

// Use sanitized output if provided
var finalOutput = outputValidation.SanitizedData ?? result.Output;
```

### Safety Rules

Safety rules implement `IAiSafetyRule` to perform specific validations. The module includes two default rules:

1. **StructuralValidationSafetyRule** - Validates input parameters and context
2. **PiiScrubbingSafetyRule** - Scrubs PII from outputs (placeholder implementation)

#### Creating Custom Safety Rules

```csharp
public class CustomSafetyRule : AiSafetyRuleBase
{
    public CustomSafetyRule(ILogger<CustomSafetyRule> logger) : base(logger)
    {
    }

    public override string Name => "Custom Validation";

    public override Task<SafetyGateResult> ValidateInputAsync(
        AiToolExecutionContext context, 
        CancellationToken cancellationToken = default)
    {
        // Implement custom input validation
        if (/* validation fails */)
        {
            return Task.FromResult(SafetyGateResult.Invalid("Validation failed"));
        }
        
        return Task.FromResult(SafetyGateResult.Valid());
    }
}
```

## Registration

Register all security components in your DI container:

```csharp
using Elsa.Copilot.Core.Security.Extensions;

// Register all security guardrails with default rules
services.AddAiSecurityGuardrails();

// Optionally add custom safety rules
services.AddAiSafetyRule<CustomSafetyRule>();

// Optionally replace the default authorization handler
services.AddAiAuthorizationHandler<CustomAuthorizationHandler>();
```

## Integration with Elsa Server

The security components are automatically registered in `ModuleRegistration.cs` of the Elsa.Copilot.Workbench project:

```csharp
internal static void RegisterModules(IServiceCollection svc)
{
    // ... other registrations ...
    
    // Register AI Security & Tenancy Guardrails
    svc.AddAiSecurityGuardrails();
}
```

## Future Integration Points

The current implementation provides TODO markers for future integration with:

1. **Elsa's Authorization System** - For actual user permission checks
2. **Elsa's Tenancy System** - For resource ownership validation
3. **Advanced PII Detection** - For production-grade PII scrubbing

## Security Best Practices

1. **Always validate tenant context** - Every AI operation must include tenant and user context
2. **Check permissions before data access** - Use `IAiAuthorizationHandler` before gathering data for LLM prompts
3. **Validate resource ownership** - Use `ValidateTenantOwnershipAsync` to ensure resources belong to the active tenant
4. **Apply safety gates consistently** - All AI tool executions should go through the safety gate
5. **Implement custom rules as needed** - Add domain-specific safety rules for your use cases
6. **Audit AI operations** - Log authorization checks and safety gate validations for security monitoring

## License

This module is part of the Elsa Copilot project and follows the same license as the parent project.
