using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Elsa.Copilot.Core.Security.SafetyGates.Rules;

/// <summary>
/// Safety rule that scrubs potential PII (Personally Identifiable Information) from data.
/// 
/// IMPORTANT: This is a placeholder implementation that demonstrates the pattern.
/// The regex patterns used here are intentionally simplistic for demonstration purposes.
/// 
/// Production implementations should:
/// - Use dedicated PII detection libraries (e.g., Microsoft Presidio, AWS Comprehend)
/// - Implement comprehensive patterns that handle format variations:
///   * Emails: RFC-compliant addresses with international characters
///   * Phone numbers: International formats, parentheses, extensions (e.g., (123) 456-7890, +1-123-456-7890)
///   * SSNs: Multiple formats (dashed, non-dashed)
/// - Consider context-aware detection to reduce false positives
/// - Support additional PII types (credit cards, addresses, names, etc.)
/// </summary>
public class PiiScrubbingSafetyRule : AiSafetyRuleBase
{
    // Simple regex patterns for common PII types (placeholder - use proper PII detection in production)
    // These patterns will have false positives and miss many valid formats
    private static readonly Regex EmailPattern = new(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b", RegexOptions.Compiled);
    private static readonly Regex PhonePattern = new(@"\b\d{3}[-.]?\d{3}[-.]?\d{4}\b", RegexOptions.Compiled);
    private static readonly Regex SsnPattern = new(@"\b\d{3}-\d{2}-\d{4}\b", RegexOptions.Compiled);

    public PiiScrubbingSafetyRule(ILogger<PiiScrubbingSafetyRule> logger) : base(logger)
    {
    }

    /// <inheritdoc />
    public override string Name => "PII Scrubbing";

    /// <inheritdoc />
    public override Task<SafetyGateResult> ValidateOutputAsync(
        AiToolExecutionContext context, 
        AiToolExecutionResult result, 
        CancellationToken cancellationToken = default)
    {
        if (result.Output == null)
        {
            return Task.FromResult(SafetyGateResult.Valid());
        }

        // If output is a string, scrub PII
        if (result.Output is string outputString)
        {
            var scrubbedOutput = ScrubPii(outputString);
            if (scrubbedOutput != outputString)
            {
                Logger.LogInformation("PII detected and scrubbed from output of tool '{ToolName}'", context.ToolName);
                return Task.FromResult(SafetyGateResult.Valid(scrubbedOutput));
            }
        }

        return Task.FromResult(SafetyGateResult.Valid(result.Output));
    }

    private string ScrubPii(string input)
    {
        // Replace emails
        input = EmailPattern.Replace(input, "[EMAIL-REDACTED]");
        
        // Replace phone numbers
        input = PhonePattern.Replace(input, "[PHONE-REDACTED]");
        
        // Replace SSNs
        input = SsnPattern.Replace(input, "[SSN-REDACTED]");
        
        return input;
    }
}
