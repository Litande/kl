using Plat4Me.DialLeadProvider.Persistent.Entities;

namespace Plat4Me.DialLeadProvider.Persistent.Repositories.Interfaces;

public interface IDataSourceRepository
{
    Task<DataSource?> GetDataSourceConfiguration(long dataSourceId, CancellationToken ct = default);
    Task<DataSource?> DataSourceUpdateDate(long dataSourceId, CancellationToken ct = default);
}