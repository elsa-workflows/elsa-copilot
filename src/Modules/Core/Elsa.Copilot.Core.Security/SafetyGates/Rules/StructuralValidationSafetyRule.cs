using Microsoft.Extensions.Logging;

namespace Elsa.Copilot.Core.Security.SafetyGates.Rules;

/// <summary>
/// Safety rule that validates structural integrity of input parameters.
/// Ensures required parameters are present and valid.
/// </summary>
public class StructuralValidationSafetyRule : AiSafetyRuleBase
{
    public StructuralValidationSafetyRule(ILogger<StructuralValidationSafetyRule> logger) : base(logger)
    {
    }

    /// <inheritdoc />
    public override string Name => "Structural Validation";

    /// <inheritdoc />
    public override Task<SafetyGateResult> ValidateInputAsync(
        AiToolExecutionContext context, 
        CancellationToken cancellationToken = default)
    {
        // Validate tool name is not empty
        if (string.IsNullOrWhiteSpace(context.ToolName))
        {
            Logger.LogWarning("Structural validation failed: Tool name is empty");
            return Task.FromResult(SafetyGateResult.Invalid("Tool name cannot be empty"));
        }

        // Validate tenant context
        if (string.IsNullOrWhiteSpace(context.TenantId))
        {
            Logger.LogWarning("Structural validation failed: Tenant ID is missing for tool '{ToolName}'", context.ToolName);
            return Task.FromResult(SafetyGateResult.Invalid("Tenant context is required"));
        }

        // Validate user context
        if (string.IsNullOrWhiteSpace(context.UserId))
        {
            Logger.LogWarning("Structural validation failed: User ID is missing for tool '{ToolName}'", context.ToolName);
            return Task.FromResult(SafetyGateResult.Invalid("User context is required"));
        }

        // Check for null values in input parameters
        var nullParameters = context.InputParameters
            .Where(kvp => kvp.Value == null)
            .Select(kvp => kvp.Key)
            .ToList();

        if (nullParameters.Any())
        {
            Logger.LogInformation(
                "Tool '{ToolName}' has null input parameters: {Parameters}",
                context.ToolName, string.Join(", ", nullParameters));
        }

        Logger.LogDebug("Structural validation passed for tool '{ToolName}'", context.ToolName);
        return Task.FromResult(SafetyGateResult.Valid());
    }
}
