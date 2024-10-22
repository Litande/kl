using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Plat4Me.Core.HealthCheck;

public class ReadinessHealthCheck : IHealthCheck
{
    private readonly Dictionary<string, ReadinessStatus> _startupCompleted = new();

    public void AddCheck(string name)
    {
        if (!_startupCompleted.TryAdd(name, ReadinessStatus.NotReady))
            throw new Exception($"Health check with name {name} was already added");
    }

    public void SetCheckStatus(string name, ReadinessStatus status)
    {
        if (!_startupCompleted.ContainsKey(name))
            throw new Exception($"Health check with name {name} was not added");

        _startupCompleted[name] = status;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (!_startupCompleted.Any())
        {
            return Task.FromResult(HealthCheckResult.Healthy("No readiness health checks were detected."));
        }

        if (_startupCompleted.Any(kvp => kvp.Value == ReadinessStatus.NotReady))
        {
            return Task.FromResult(HealthCheckResult.Degraded("Service NOT Ready",
                data: _startupCompleted
                    .ToDictionary(k => k.Key, v => (object) new {State = v.Value.ToString()})));
        }


        return Task.FromResult(HealthCheckResult.Healthy("Service Ready.",
            data: _startupCompleted
                .ToDictionary(k => k.Key, v => (object) new {State = v.Value.ToString()})));
    }
}