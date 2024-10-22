using KL.Engine.Rule.Models.Responses;

namespace KL.Engine.Rule.Handlers.Contracts;

public interface IGetNextFromLeadQueueHandler
{
    Task<GetNextLeadResponse?> Process(long clientId, long queueId, IReadOnlyCollection<long>? agentIds, CancellationToken ct = default);
}
