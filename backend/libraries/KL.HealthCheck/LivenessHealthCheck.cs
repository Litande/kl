using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Plat4Me.Core.HealthCheck;

public class LivenessHealthCheck : IHealthCheck
{
    private readonly Dictionary<string, LivenessStatus> _startupCompleted = new();

    public void AddCheck(string name, LivenessStatus status)
    {
        if (!_startupCompleted.TryAdd(name, status))
            throw new Exception($"Health check with name {name} was already added");
    }

    public void SetCheckStatus(string name, LivenessStatus status)
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
            return Task.FromResult(HealthCheckResult.Healthy("No liveness health checks were detected."));
        }

        if (_startupCompleted.All(kvp => kvp.Value == LivenessStatus.Alive))
        {
            return Task.FromResult(HealthCheckResult.Healthy("Service works correctly",
                _startupCompleted
                    .ToDictionary(k => k.Key, v => (object) new {State = v.Value.ToString()})));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy("Service errors detected",
            data: _startupCompleted
                .ToDictionary(k => k.Key, v => (object) new {State = v.Value.ToString()})));
    }
}