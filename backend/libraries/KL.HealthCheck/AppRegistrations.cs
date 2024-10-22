using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace Plat4Me.Core.HealthCheck;

public static class AppRegistrations
{
    public static IHealthChecksBuilder AddCoreHealthCheck(this IServiceCollection services) =>
        services
            .AddSingleton<ReadinessHealthCheck>()
            .AddSingleton<LivenessHealthCheck>()
            .AddHealthChecks()
            .AddCheck<ReadinessHealthCheck>("Startup", tags: new[] {"Readiness"})
            .AddCheck<LivenessHealthCheck>("Running", tags: new[] {"Liveness"});

    public static void MapCoreHealthCheck(this WebApplication app)
    {
        app.MapHealthChecks("/healthz/ready", new HealthCheckOptions
        {
            Predicate = healthCheck => healthCheck.Tags.Contains("Readiness")
        });

        app.MapHealthChecks("/healthz/live", new HealthCheckOptions
        {
            Predicate = healthCheck => healthCheck.Tags.Contains("Liveness")
        });
    }
}