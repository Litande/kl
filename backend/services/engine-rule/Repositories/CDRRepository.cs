using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KL.Engine.Rule.Repositories;

public class CDRRepository : ICDRRepository
{
    private readonly KlDbContext _context;

    public CDRRepository(KlDbContext context)
    {
        _context = context;
    }

    public async Task<CallDetailRecord?> GetByLeadId(
        long leadId,
        CancellationToken ct = default)
    {
        var entity = await _context.CallDetailRecords
            .Where(r => r.LeadId == leadId)
            .FirstOrDefaultAsync(ct);

        return entity;
    }

    public int GetLeadTotalCallsCount(
        long leadId,
        DateTimeOffset? fromDateTime = null)
    {
        var q = _context.CallDetailRecords
            .Where(r => r.LeadId == leadId
                        && r.CallHangupAt.HasValue);

        if (fromDateTime.HasValue)
            q = q.Where(r => r.UserAnswerAt > fromDateTime);

        var totalCalls = q.Count();

        return totalCalls;
    }

    public long GetLeadTotalCallsSeconds(
        long leadId,
        DateTimeOffset? fromDateTime = null)
    {
        var q = _context.CallDetailRecords
            .Where(r => r.LeadId == leadId
                        && r.UserAnswerAt.HasValue
                        && r.CallHangupAt.HasValue);

        if (fromDateTime.HasValue)
            q = q.Where(r => r.UserAnswerAt > fromDateTime);

        var totalCalls = q
            .Sum(r => r.CallDuration.GetValueOrDefault());

        return totalCalls;
    }

    public async Task<IReadOnlyCollection<CallDetailRecord>> GetCallDetailsForCalculationsByClient(
        long clientId,
        DateTimeOffset startFrom,
        IEnumerable<LeadStatusTypes> statuses,
        CancellationToken ct = default)
    {
        var callDetailRecords = await _context.CallDetailRecords
            .Include(x => x.Lead)
            .Where(x => x.ClientId == clientId
                        && x.LeadId.HasValue
                        && !string.IsNullOrEmpty(x.LeadCountry)
                        && x.OriginatedAt >= startFrom
                        && statuses.Contains(x.Lead!.Status)
            )
            .ToArrayAsync(ct);

        return callDetailRecords;
    }
}