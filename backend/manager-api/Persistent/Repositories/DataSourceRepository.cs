using Microsoft.EntityFrameworkCore;
using Plat4Me.DialClientApi.Persistent.Entities;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialClientApi.Persistent.Repositories;

public class DataSourceRepository : IDataSourceRepository
{
    private readonly DialDbContext _context;

    public DataSourceRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<DataSource>> GetAllDataSource(CancellationToken ct = default)
    {
        return await _context.DataSources.AsNoTracking().ToListAsync(ct);
    }
}