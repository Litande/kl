using KL.Caller.Leads.Enums;
using KL.Caller.Leads.Models;

namespace KL.Caller.Leads.Repositories;

public interface IQueueLeadsCacheRepository
{
    Task<QueueLeadCache?> GetById(long clientId, long queueId, long leadId);
    Task UpdateStatus(long clientId, long queueId, long leadId, LeadSystemStatusTypes? systemStatus, LeadStatusTypes? status = null, CancellationToken ct = default);
}
