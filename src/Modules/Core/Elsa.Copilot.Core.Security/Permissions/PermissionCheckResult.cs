namespace Elsa.Copilot.Core.Security.Permissions;

/// <summary>
/// Defines the result of a permission check.
/// </summary>
public class PermissionCheckResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the permission check passed.
    /// </summary>
    public bool IsAuthorized { get; set; }

    /// <summary>
    /// Gets or sets the reason for denial if not authorized.
    /// </summary>
    public string? DenialReason { get; set; }

    /// <summary>
    /// Creates a successful permission check result.
    /// </summary>
    public static PermissionCheckResult Authorized() => new() { IsAuthorized = true };

    /// <summary>
    /// Creates a failed permission check result with a reason.
    /// </summary>
    public static PermissionCheckResult Denied(string reason) => new() { IsAuthorized = false, DenialReason = reason };
}
