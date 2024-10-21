using Plat4Me.DialLeadProvider.Persistent.Entities;
using Plat4Me.DialLeadProvider.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialLeadProvider.Persistent.Repositories;

public class LeadDataSourceMapRepository : ILeadDataSourceMapRepository
{
    private readonly DialDbContext _context;

    public LeadDataSourceMapRepository(DialDbContext context)
    {
        _context = context;
    }

    public IEnumerable<LeadDataSourceMap> GetLeadDataSourceMap()
    {
        var dataSourceMap = _context.LeadDataSourceMaps
            .ToList();

        return dataSourceMap;
    }
}