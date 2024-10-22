using KL.Engine.Rule.Models;

namespace KL.Engine.Rule.Services.Contracts;

public interface ILeadsQueueStore
{
    public void UpdateAll(long clientId, IReadOnlyCollection<TrackedLead> leads);
    TrackedLead? PopNextLead(long clientId, long queueId, IReadOnlyCollection<long>? agentIds);
    void SetLeadManualScore(long clientId, long leadId, long score);
    bool TryRemoveLead(long clientId, long leadId);
}
