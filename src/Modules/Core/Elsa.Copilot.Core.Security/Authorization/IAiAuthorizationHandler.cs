using Elsa.Copilot.Core.Security.Permissions;

namespace Elsa.Copilot.Core.Security.Authorization;

/// <summary>
/// Represents the context for an AI authorization request.
/// Contains information about the user, tenant, and requested operation.
/// </summary>
public class AiAuthorizationContext
{
    /// <summary>
    /// Gets or sets the tenant identifier for the current operation.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier performing the operation.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets the required permission scope for the operation.
    /// </summary>
    public string RequiredScope { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional context data specific to the operation.
    /// </summary>
    public Dictionary<string, object> AdditionalContext { get; set; } = new();
}

/// <summary>
/// Handles authorization for AI operations, enforcing tenancy boundaries and permission checks.
/// </summary>
public interface IAiAuthorizationHandler
{
    /// <summary>
    /// Validates that the current context has the required permissions for an AI operation.
    /// </summary>
    /// <param name="context">The authorization context containing user, tenant, and scope information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the permission check.</returns>
    Task<PermissionCheckResult> AuthorizeAsync(AiAuthorizationContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that a specific resource belongs to the specified tenant.
    /// Prevents cross-tenant data leakage.
    /// </summary>
    /// <param name="tenantId">The tenant identifier to validate against.</param>
    /// <param name="resourceId">The resource identifier to check.</param>
    /// <param name="resourceType">The type of resource being validated.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the resource belongs to the tenant, false otherwise.</returns>
    Task<bool> ValidateTenantOwnershipAsync(string tenantId, string resourceId, string resourceType, CancellationToken cancellationToken = default);
}
