using Plat4Me.DialRuleEngine.Application.Models;

namespace Plat4Me.DialRuleEngine.Application.Handlers.Contracts;

public interface ILeadQueueStoreUpdateHandler
{
    Task Process(long clientId, IReadOnlyCollection<TrackedLead> trackedLeads, CancellationToken ct = default);
}
