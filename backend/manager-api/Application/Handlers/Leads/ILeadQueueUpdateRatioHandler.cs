using Plat4Me.DialClientApi.Persistent.Entities;

namespace Plat4Me.DialClientApi.Application.Handlers.Leads;

public interface ILeadQueueUpdateRatioHandler
{
    Task<LeadQueue?> Handle(long clientId, long leadQueueId, double ratio, CancellationToken ct = default);
}
