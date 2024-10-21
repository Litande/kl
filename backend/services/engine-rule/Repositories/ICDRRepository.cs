using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Models.Entities;

namespace Plat4Me.DialRuleEngine.Application.Repositories;

public interface ICDRRepository
{
    Task<CallDetailRecord?> GetByLeadId(long leadId, CancellationToken ct = default);
    int GetLeadTotalCallsCount(long leadId, DateTimeOffset? fromDateTime = null);
    long GetLeadTotalCallsSeconds(long leadId, DateTimeOffset? fromDateTime = null);
    Task<IReadOnlyCollection<CallDetailRecord>> GetCallDetailsForCalculationsByClient(long clientId, DateTimeOffset startFrom, IEnumerable<LeadStatusTypes> statuses, CancellationToken ct = default);
}
