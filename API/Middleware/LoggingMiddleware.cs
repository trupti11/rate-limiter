using Microsoft.AspNetCore.Builder;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace RateLimiter.API.Middleware;

/// <summary>
/// Logging middleware
/// </summary>
[ExcludeFromCodeCoverage]
public static class LoggingMiddleware
{
    /// <summary>
    /// Adds the logging middleware through Program.cs
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddLoggingMiddleware(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .WriteTo.Console()
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
                .Enrich.WithMachineName()
                .Enrich.WithCorrelationId()
                .Enrich.FromLogContext());
        return builder;
    }
}

