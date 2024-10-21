using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Models;

namespace Plat4Me.DialRuleEngine.Application.Repositories;

public interface IQueueLeadsCacheRepository
{
    Task<IReadOnlyCollection<QueueLeadCache>> GetAll(long clientId, CancellationToken ct = default);
    Task<QueueLeadCache?> GetById(long clientId, long queueId, long leadId);
    Task<QueueLeadCache?> GetById(long clientId, long leadId);
    Task<long?> GetQueueId(long clientId, long leadId);
    Task Remove(long clientId, long? queueId, long leadId, CancellationToken ct = default);
    Task UpdateAll(long clientId, IEnumerable<TrackedLead> leadsUpdate, CancellationToken ct = default);
    Task UpdateSystemStatus(long clientId, long queueId, long leadId, LeadSystemStatusTypes? systemStatus);
    Task UpdateScore(long clientId, long queueId, long leadId, long score);
    Task ValidateLeadCache(IEnumerable<LeadStatusDto> leads, CancellationToken ct = default);
}
