using Plat4Me.DialLeadCaller.Application.Enums;
using Plat4Me.DialLeadCaller.Application.Models.Entities;

namespace Plat4Me.DialLeadCaller.Application.Repositories;

public interface ILeadRepository
{
    Task<Lead?> UpdateStatusAndGet(long clientId, long leadId, LeadSystemStatusTypes? systemStatus, LeadStatusTypes? status = null, CancellationToken ct = default);
    Task UpdateSystemStatuses(long clientId, IEnumerable<long> leadIds, LeadSystemStatusTypes? systemStatus, CancellationToken ct = default);
    Task<Lead?> GetLeadById(long clientId, long leadId, CancellationToken ct = default);
    Task<Lead?> SaveFeedbackAndGet(long clientId, long agentId, bool isGenerated, long leadId, LeadSystemStatusTypes? systemStatus, LeadStatusTypes status, DateTimeOffset? remindOn = null, CancellationToken ct = default);
}
