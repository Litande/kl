using Microsoft.EntityFrameworkCore;
using Plat4Me.DialLeadProvider.Application.Enums;
using Plat4Me.DialLeadProvider.Persistent.Entities;
using Plat4Me.DialLeadProvider.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialLeadProvider.Persistent.Repositories;

public class DataSourceRepository : IDataSourceRepository
{
    private readonly DialDbContext _context;

    public DataSourceRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<DataSource?> GetDataSourceConfiguration(
        long dataSourceId,
        CancellationToken ct = default)
    {
        var entity = await _context.DataSources
            .Where(i => i.Id == dataSourceId
                        && i.Status == DataSourceStatusType.Enable)
            .FirstOrDefaultAsync(ct);

        return entity;
    }

    public async Task<DataSource?> DataSourceUpdateDate(
        long dataSourceId,
        CancellationToken ct = default)
    {
        var entity = await _context.DataSources
            .Where(r => r.Id == dataSourceId)
            .FirstOrDefaultAsync(ct);

        if (entity is null) return null;

        entity.MinUpdateDate = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(ct);

        return entity;
    }
}