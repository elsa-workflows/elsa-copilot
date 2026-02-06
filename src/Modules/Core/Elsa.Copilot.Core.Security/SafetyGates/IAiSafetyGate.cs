namespace Elsa.Copilot.Core.Security.SafetyGates;

/// <summary>
/// Represents the context for an AI tool execution.
/// </summary>
public class AiToolExecutionContext
{
    /// <summary>
    /// Gets or sets the name of the tool being executed.
    /// </summary>
    public string ToolName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the input parameters for the tool.
    /// </summary>
    public Dictionary<string, object?> InputParameters { get; set; } = new();

    /// <summary>
    /// Gets or sets the tenant identifier for the execution.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier performing the execution.
    /// </summary>
    public string? UserId { get; set; }
}

/// <summary>
/// Represents the result of an AI tool execution.
/// </summary>
public class AiToolExecutionResult
{
    /// <summary>
    /// Gets or sets the output data from the tool execution.
    /// </summary>
    public object? Output { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the execution was successful.
    /// </summary>
    public bool IsSuccessful { get; set; }

    /// <summary>
    /// Gets or sets error message if execution failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Result of a safety gate validation.
/// </summary>
public class SafetyGateResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the validation passed.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets the validation message or reason for failure.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the scrubbed or modified data (if applicable).
    /// </summary>
    public object? SanitizedData { get; set; }

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    public static SafetyGateResult Valid(object? sanitizedData = null) => new() 
    { 
        IsValid = true, 
        SanitizedData = sanitizedData 
    };

    /// <summary>
    /// Creates a failed validation result.
    /// </summary>
    public static SafetyGateResult Invalid(string message) => new() 
    { 
        IsValid = false, 
        Message = message 
    };
}

/// <summary>
/// Server-side safety gate for AI tool execution.
/// Intercepts and validates inputs and outputs before they are processed.
/// </summary>
public interface IAiSafetyGate
{
    /// <summary>
    /// Validates input parameters before tool execution.
    /// </summary>
    /// <param name="context">The execution context with tool inputs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The validation result, potentially with sanitized data.</returns>
    Task<SafetyGateResult> ValidateInputAsync(AiToolExecutionContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates or scrubs output data before returning to the orchestrator.
    /// </summary>
    /// <param name="context">The execution context.</param>
    /// <param name="result">The tool execution result.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The validation result, potentially with scrubbed output.</returns>
    Task<SafetyGateResult> ValidateOutputAsync(AiToolExecutionContext context, AiToolExecutionResult result, CancellationToken cancellationToken = default);
}
