using Plat4Me.DialLeadCaller.Application.Enums;
using Plat4Me.DialLeadCaller.Application.Models;

namespace Plat4Me.DialLeadCaller.Application.Repositories;

public interface IQueueLeadsCacheRepository
{
    Task<QueueLeadCache?> GetById(long clientId, long queueId, long leadId);
    Task UpdateStatus(long clientId, long queueId, long leadId, LeadSystemStatusTypes? systemStatus, LeadStatusTypes? status = null, CancellationToken ct = default);
}
