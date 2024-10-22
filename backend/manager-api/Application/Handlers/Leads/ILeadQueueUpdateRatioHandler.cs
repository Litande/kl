using KL.Manager.API.Persistent.Entities;

namespace KL.Manager.API.Application.Handlers.Leads;

public interface ILeadQueueUpdateRatioHandler
{
    Task<LeadQueue?> Handle(long clientId, long leadQueueId, double ratio, CancellationToken ct = default);
}
