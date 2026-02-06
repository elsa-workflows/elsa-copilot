namespace Elsa.Copilot.Core.Security.Permissions;

/// <summary>
/// Defines the granular permission scopes for AI-driven actions in Elsa Copilot.
/// These scopes integrate with Elsa's standard authorization policy system.
/// </summary>
public static class AiPermissionScopes
{
    /// <summary>
    /// Required to gather workflow/instance state for AI analysis.
    /// Allows reading workflow definitions, instances, and runtime state.
    /// </summary>
    public const string Read = "ai:read";

    /// <summary>
    /// Required for the AI to generate and submit change proposals.
    /// Allows creating workflow modification proposals that require user approval.
    /// </summary>
    public const string Propose = "ai:propose";

    /// <summary>
    /// Required to generate runtime failure explanations and diagnostics.
    /// Allows analyzing errors, bookmarks, and execution traces.
    /// </summary>
    public const string Diagnose = "ai:diagnose";

    /// <summary>
    /// Required for managing AI provider and tool configurations.
    /// Allows administrative access to AI system settings.
    /// </summary>
    public const string Admin = "ai:admin";

    /// <summary>
    /// Gets all defined AI permission scopes.
    /// </summary>
    public static readonly string[] AllScopes = new[]
    {
        Read,
        Propose,
        Diagnose,
        Admin
    };
}
