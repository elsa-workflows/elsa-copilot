using Elsa.Copilot.Core.Security.Authorization;
using Elsa.Copilot.Core.Security.SafetyGates;
using Elsa.Copilot.Core.Security.SafetyGates.Rules;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa.Copilot.Core.Security.Extensions;

/// <summary>
/// Extension methods for registering AI security services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all AI security and guardrail services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAiSecurityGuardrails(this IServiceCollection services)
    {
        // Register authorization handler
        services.AddScoped<IAiAuthorizationHandler, DefaultAiAuthorizationHandler>();

        // Register safety gate
        services.AddScoped<IAiSafetyGate, DefaultAiSafetyGate>();

        // Register default safety rules
        services.AddScoped<IAiSafetyRule, StructuralValidationSafetyRule>();
        services.AddScoped<IAiSafetyRule, PiiScrubbingSafetyRule>();

        return services;
    }

    /// <summary>
    /// Registers a custom safety rule.
    /// </summary>
    /// <typeparam name="TRule">The type of the safety rule to register.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAiSafetyRule<TRule>(this IServiceCollection services)
        where TRule : class, IAiSafetyRule
    {
        services.AddScoped<IAiSafetyRule, TRule>();
        return services;
    }

    /// <summary>
    /// Registers a custom authorization handler, replacing any existing handler.
    /// </summary>
    /// <typeparam name="THandler">The type of the authorization handler to register.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAiAuthorizationHandler<THandler>(this IServiceCollection services)
        where THandler : class, IAiAuthorizationHandler
    {
        // Remove existing handler registration if present
        var existingDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IAiAuthorizationHandler));
        if (existingDescriptor != null)
        {
            services.Remove(existingDescriptor);
        }
        
        // Add the custom handler
        services.AddScoped<IAiAuthorizationHandler, THandler>();
        return services;
    }
}
