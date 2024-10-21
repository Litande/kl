using Plat4Me.DialClientApi.Application.Models.Messages.RuleEngine;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialClientApi.Application.Handlers;

public class RuleEngineRunHandler : IRuleEngineRunHandler
{
    private readonly IRuleEngineCacheRepository _ruleEngineCacheRepository;

    public RuleEngineRunHandler(IRuleEngineCacheRepository ruleEngineCacheRepository)
    {
        _ruleEngineCacheRepository = ruleEngineCacheRepository;
    }

    public Task Process(RuleEngineRunMessage message, CancellationToken ct = default)
    {
        _ruleEngineCacheRepository.ResetCache();

        return Task.CompletedTask;
    }
}
