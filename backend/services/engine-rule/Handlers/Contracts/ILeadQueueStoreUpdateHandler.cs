using KL.Engine.Rule.Models;

namespace KL.Engine.Rule.Handlers.Contracts;

public interface ILeadQueueStoreUpdateHandler
{
    Task Process(long clientId, IReadOnlyCollection<TrackedLead> trackedLeads, CancellationToken ct = default);
}
