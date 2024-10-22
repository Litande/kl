using KL.Manager.API.Persistent.Entities;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KL.Manager.API.Persistent.Repositories;

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