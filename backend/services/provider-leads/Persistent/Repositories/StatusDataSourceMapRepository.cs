using KL.Provider.Leads.Application.Enums;
using KL.Provider.Leads.Persistent.Entities;
using KL.Provider.Leads.Persistent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KL.Provider.Leads.Persistent.Repositories;

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