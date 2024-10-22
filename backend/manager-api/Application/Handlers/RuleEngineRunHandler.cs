using KL.Manager.API.Application.Models.Messages.RuleEngine;
using KL.Manager.API.Persistent.Repositories.Interfaces;

namespace KL.Manager.API.Application.Handlers;

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
