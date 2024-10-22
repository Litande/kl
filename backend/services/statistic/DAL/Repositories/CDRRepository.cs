using KL.Statistics.Application.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KL.Statistics.DAL.Repositories;

public class CdrRepository : ICdrRepository
{
    private readonly KlDbContext _context;

    public CdrRepository(KlDbContext context)
    {
        _context = context;
    }

    public async Task<CallDetailRecord?> GetById(
        long currentClientId,
        long cdrId,
        CancellationToken ct = default)
    {
        return await _context.CallDetailRecords
            .Where(r => r.ClientId == currentClientId
                        && r.Id == cdrId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyCollection<CallDetailRecord>> GetCallsByPeriod(
        long clientId,
        DateTimeOffset fromDateTime,
        DateTimeOffset toDateTime,
        CancellationToken ct = default)
    {
        var calls = await _context.CallDetailRecords.AsNoTracking()
            .Where(c => c.ClientId == clientId
                        && c.OriginatedAt > fromDateTime
                        && c.OriginatedAt <= toDateTime)
            .ToListAsync(ct);

        return calls;
    }

}
