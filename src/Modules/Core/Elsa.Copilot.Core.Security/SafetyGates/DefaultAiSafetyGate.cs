using Microsoft.Extensions.Logging;

namespace Elsa.Copilot.Core.Security.SafetyGates;

/// <summary>
/// Default implementation of <see cref="IAiSafetyGate"/>.
/// Applies a chain of safety rules to validate inputs and outputs.
/// </summary>
public class DefaultAiSafetyGate : IAiSafetyGate
{
    private readonly IEnumerable<IAiSafetyRule> _safetyRules;
    private readonly ILogger<DefaultAiSafetyGate> _logger;

    public DefaultAiSafetyGate(
        IEnumerable<IAiSafetyRule> safetyRules,
        ILogger<DefaultAiSafetyGate> logger)
    {
        _safetyRules = safetyRules;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<SafetyGateResult> ValidateInputAsync(AiToolExecutionContext context, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validating input for tool '{ToolName}'", context.ToolName);

        // Apply all safety rules in sequence
        foreach (var rule in _safetyRules)
        {
            var result = await rule.ValidateInputAsync(context, cancellationToken);
            if (!result.IsValid)
            {
                _logger.LogWarning(
                    "Input validation failed for tool '{ToolName}' by rule '{RuleName}': {Message}",
                    context.ToolName, rule.Name, result.Message);
                return result;
            }

            // If the rule modified the data, update the context
            if (result.SanitizedData != null)
            {
                _logger.LogInformation(
                    "Input sanitized for tool '{ToolName}' by rule '{RuleName}'",
                    context.ToolName, rule.Name);
            }
        }

        _logger.LogInformation("Input validation passed for tool '{ToolName}'", context.ToolName);
        return SafetyGateResult.Valid();
    }

    /// <inheritdoc />
    public async Task<SafetyGateResult> ValidateOutputAsync(AiToolExecutionContext context, AiToolExecutionResult result, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validating output for tool '{ToolName}'", context.ToolName);

        object? currentOutput = result.Output;

        // Apply all safety rules in sequence
        foreach (var rule in _safetyRules)
        {
            var validationResult = await rule.ValidateOutputAsync(context, result, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning(
                    "Output validation failed for tool '{ToolName}' by rule '{RuleName}': {Message}",
                    context.ToolName, rule.Name, validationResult.Message);
                return validationResult;
            }

            // If the rule scrubbed the output, use the sanitized version
            if (validationResult.SanitizedData != null)
            {
                currentOutput = validationResult.SanitizedData;
                _logger.LogInformation(
                    "Output scrubbed for tool '{ToolName}' by rule '{RuleName}'",
                    context.ToolName, rule.Name);
            }
        }

        _logger.LogInformation("Output validation passed for tool '{ToolName}'", context.ToolName);
        return SafetyGateResult.Valid(currentOutput);
    }
}
