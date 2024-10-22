using KL.Manager.API.Application.Enums;
using KL.Manager.API.Persistent.Clients;
using KL.Manager.API.Persistent.Repositories.Interfaces;

namespace KL.Manager.API.Persistent.Repositories;

public class RuleEngineCacheRepository : IRuleEngineCacheRepository
{
    private readonly IServiceProvider _serviceProvider;

    private Dictionary<long, Dictionary<RuleGroupTypes, string>> _conditions = new();
    private Dictionary<long, Dictionary<RuleGroupTypes, string>> _actions = new();

    public RuleEngineCacheRepository(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<string> GetConditions(long clientId, RuleGroupTypes ruleType, CancellationToken ct = default)
    {
        if (!_conditions.TryGetValue(clientId, out var clientConditions))
        {
            clientConditions = await UpdateClientConditions(clientId, ct);
        }

        if (!clientConditions.TryGetValue(ruleType, out var result))
            throw new ArgumentException("Actions not found");
        return result;
    }

    public async Task<string> GetActions(long clientId, RuleGroupTypes ruleType, CancellationToken ct = default)
    {
        if (!_actions.TryGetValue(clientId, out var clientActions))
        {
            clientActions = await UpdateClientActions(clientId, ct);
        }

        if (!clientActions.TryGetValue(ruleType, out var result))
            throw new ArgumentException("Actions not found");
        return result;
    }

    public void ResetCache()
    {
        _conditions = new();
        _actions = new();
    }

    private async Task<Dictionary<RuleGroupTypes, string>> UpdateClientConditions(long clientId, CancellationToken ct = default)
    {
        var client = _serviceProvider.GetRequiredService<IRuleEngineClient>();
        var conditions = await client.GetConditions(clientId, ct);
        _conditions[clientId] = conditions;
        return conditions;
    }

    private async Task<Dictionary<RuleGroupTypes, string>> UpdateClientActions(long clientId, CancellationToken ct = default)
    {
        var client = _serviceProvider.GetRequiredService<IRuleEngineClient>();
        var actions = await client.GetActions(clientId, ct);
        _actions[clientId] = actions;
        return actions;
    }
}
