using Plat4Me.DialRuleEngine.Application.Models.Responses;

namespace Plat4Me.DialRuleEngine.Application.Handlers.Contracts;

public interface IGetNextFromLeadQueueHandler
{
    Task<GetNextLeadResponse?> Process(long clientId, long queueId, IReadOnlyCollection<long>? agentIds, CancellationToken ct = default);
}
