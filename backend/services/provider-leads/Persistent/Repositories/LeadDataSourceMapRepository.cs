using KL.Provider.Leads.Persistent.Entities;
using KL.Provider.Leads.Persistent.Repositories.Interfaces;

namespace KL.Provider.Leads.Persistent.Repositories;

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