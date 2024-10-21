using Microsoft.EntityFrameworkCore;
using Plat4Me.DialLeadProvider.Application.Enums;
using Plat4Me.DialLeadProvider.Persistent.Entities;
using Plat4Me.DialLeadProvider.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialLeadProvider.Persistent.Repositories;

public class StatusDataSourceMapRepository : IStatusDataSourceMapRepository
{
    private readonly DialDbContext _context;

    public StatusDataSourceMapRepository(DialDbContext context)
    {
        _context = context;
    }

    public IEnumerable<StatusDataSourceMap> GetStatusDataSourceMap()
    {
        var statusDataSources = _context.StatusDataSourceMaps.ToList()
            .GroupBy(i => i.ExternalStatusId).Select(x => x.First());

        return statusDataSources;
    }

    public async Task<StatusDataSourceMap?> GetByStatus(
        LeadStatusTypes status,
        CancellationToken ct = default)
    {
        var result = await _context.StatusDataSourceMaps
            .FirstOrDefaultAsync(x => x.Status == status, ct);

        return result;
    }
}