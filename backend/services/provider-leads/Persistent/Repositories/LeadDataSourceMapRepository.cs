using KL.Provider.Leads.Persistent.Entities;
using KL.Provider.Leads.Persistent.Repositories.Interfaces;

namespace KL.Provider.Leads.Persistent.Repositories;

public class LeadDataSourceMapRepository : ILeadDataSourceMapRepository
{
    private readonly KlDbContext _context;

    public LeadDataSourceMapRepository(KlDbContext context)
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