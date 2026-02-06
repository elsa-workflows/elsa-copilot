using Microsoft.Extensions.Logging;

namespace Elsa.Copilot.Core.Security.SafetyGates;

/// <summary>
/// Base interface for implementing specific safety rules.
/// Safety rules can be chained to perform multiple validations.
/// </summary>
public interface IAiSafetyRule
{
    /// <summary>
    /// Gets the name of the safety rule.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Validates input data according to the rule.
    /// </summary>
    Task<SafetyGateResult> ValidateInputAsync(AiToolExecutionContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates output data according to the rule.
    /// </summary>
    Task<SafetyGateResult> ValidateOutputAsync(AiToolExecutionContext context, AiToolExecutionResult result, CancellationToken cancellationToken = default);
}

/// <summary>
/// Base class for implementing safety rules with common functionality.
/// </summary>
public abstract class AiSafetyRuleBase : IAiSafetyRule
{
    protected readonly ILogger Logger;

    protected AiSafetyRuleBase(ILogger logger)
    {
        Logger = logger;
    }

    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public virtual Task<SafetyGateResult> ValidateInputAsync(AiToolExecutionContext context, CancellationToken cancellationToken = default)
    {
        // Default implementation: pass through
        return Task.FromResult(SafetyGateResult.Valid());
    }

    /// <inheritdoc />
    public virtual Task<SafetyGateResult> ValidateOutputAsync(AiToolExecutionContext context, AiToolExecutionResult result, CancellationToken cancellationToken = default)
    {
        // Default implementation: pass through
        return Task.FromResult(SafetyGateResult.Valid(result.Output));
    }
}
