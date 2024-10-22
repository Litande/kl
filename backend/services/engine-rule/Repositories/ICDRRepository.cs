using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models.Entities;

namespace KL.Engine.Rule.Repositories;

public interface ICDRRepository
{
    Task<CallDetailRecord?> GetByLeadId(long leadId, CancellationToken ct = default);
    int GetLeadTotalCallsCount(long leadId, DateTimeOffset? fromDateTime = null);
    long GetLeadTotalCallsSeconds(long leadId, DateTimeOffset? fromDateTime = null);
    Task<IReadOnlyCollection<CallDetailRecord>> GetCallDetailsForCalculationsByClient(long clientId, DateTimeOffset startFrom, IEnumerable<LeadStatusTypes> statuses, CancellationToken ct = default);
}
