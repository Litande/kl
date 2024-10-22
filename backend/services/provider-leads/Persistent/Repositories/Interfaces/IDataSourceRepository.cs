using KL.Provider.Leads.Persistent.Entities;

namespace KL.Provider.Leads.Persistent.Repositories.Interfaces;

public interface IDataSourceRepository
{
    Task<DataSource?> GetDataSourceConfiguration(long dataSourceId, CancellationToken ct = default);
    Task<DataSource?> DataSourceUpdateDate(long dataSourceId, CancellationToken ct = default);
}