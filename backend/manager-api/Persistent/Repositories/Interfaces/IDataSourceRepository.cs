using Plat4Me.DialClientApi.Persistent.Entities;

namespace Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

public interface IDataSourceRepository
{
    Task<IReadOnlyCollection<DataSource>> GetAllDataSource(CancellationToken ct = default);
}