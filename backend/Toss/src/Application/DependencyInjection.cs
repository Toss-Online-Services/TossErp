using System.Reflection;
using Toss.Application.Common.Behaviours;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for registering Application layer services.
/// Implements CQRS pattern using MediatR with comprehensive behavioral pipeline.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Application layer services including CQRS handlers, validators, and behavioral pipeline.
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    /// <remarks>
    /// Configures the following services:
    /// <list type="bullet">
    /// <item><description>AutoMapper for object-to-object mapping</description></item>
    /// <item><description>FluentValidation validators from assembly</description></item>
    /// <item><description>MediatR with behavioral pipeline:
    ///   <list type="number">
    ///   <item>LoggingBehaviour - Pre-processor that logs all requests with user context</item>
    ///   <item>UnhandledExceptionBehaviour - Catches and logs unhandled exceptions</item>
    ///   <item>AuthorizationBehaviour - Enforces role and policy-based authorization</item>
    ///   <item>ValidationBehaviour - Validates requests using FluentValidation</item>
    ///   <item>PerformanceBehaviour - Monitors and logs slow requests (>500ms)</item>
    ///   </list>
    /// </description></item>
    /// </list>
    /// Pipeline execution order: Logging → Exception Handling → Authorization → Validation → Performance → Handler
    /// </remarks>
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        // Object mapping configuration
        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Validation configuration
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // CQRS configuration with behavioral pipeline
        builder.Services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenRequestPreProcessor(typeof(LoggingBehaviour<>));
            cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
        });
    }
}
