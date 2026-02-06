using Elsa.Copilot.Core.Security.Permissions;
using Microsoft.Extensions.Logging;

namespace Elsa.Copilot.Core.Security.Authorization;

/// <summary>
/// Default implementation of <see cref="IAiAuthorizationHandler"/>.
/// Enforces tenancy boundaries and validates AI permission scopes.
/// </summary>
public class DefaultAiAuthorizationHandler : IAiAuthorizationHandler
{
    private readonly ILogger<DefaultAiAuthorizationHandler> _logger;

    public DefaultAiAuthorizationHandler(ILogger<DefaultAiAuthorizationHandler> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<PermissionCheckResult> AuthorizeAsync(AiAuthorizationContext context, CancellationToken cancellationToken = default)
    {
        // Validate tenant context
        if (string.IsNullOrWhiteSpace(context.TenantId))
        {
            _logger.LogWarning("AI authorization denied: No tenant context provided");
            return Task.FromResult(PermissionCheckResult.Denied("No tenant context provided"));
        }

        // Validate user context
        if (string.IsNullOrWhiteSpace(context.UserId))
        {
            _logger.LogWarning("AI authorization denied: No user context provided");
            return Task.FromResult(PermissionCheckResult.Denied("No user context provided"));
        }

        // Validate required scope
        if (string.IsNullOrWhiteSpace(context.RequiredScope))
        {
            _logger.LogWarning("AI authorization denied: No permission scope specified");
            return Task.FromResult(PermissionCheckResult.Denied("No permission scope specified"));
        }

        // Check if the scope is valid
        if (!AiPermissionScopes.AllScopes.Contains(context.RequiredScope))
        {
            _logger.LogWarning("AI authorization denied: Invalid permission scope '{Scope}'", context.RequiredScope);
            return Task.FromResult(PermissionCheckResult.Denied($"Invalid permission scope: {context.RequiredScope}"));
        }

        // TODO: Integrate with Elsa's authorization system to check actual user permissions
        // For now, we log and allow - real implementation would check against user roles/claims
        _logger.LogInformation(
            "AI authorization check passed for user '{UserId}' in tenant '{TenantId}' with scope '{Scope}'",
            context.UserId, context.TenantId, context.RequiredScope);

        return Task.FromResult(PermissionCheckResult.Authorized());
    }

    /// <inheritdoc />
    public Task<bool> ValidateTenantOwnershipAsync(string tenantId, string resourceId, string resourceType, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            _logger.LogWarning("Tenant ownership validation failed: No tenant ID provided");
            return Task.FromResult(false);
        }

        if (string.IsNullOrWhiteSpace(resourceId))
        {
            _logger.LogWarning("Tenant ownership validation failed: No resource ID provided");
            return Task.FromResult(false);
        }

        // TODO: Integrate with Elsa's tenant system to validate resource ownership
        // For now, we log and allow - real implementation would query the resource
        // from the database and verify its tenant association
        _logger.LogInformation(
            "Tenant ownership validated for resource '{ResourceId}' of type '{ResourceType}' in tenant '{TenantId}'",
            resourceId, resourceType, tenantId);

        return Task.FromResult(true);
    }
}
