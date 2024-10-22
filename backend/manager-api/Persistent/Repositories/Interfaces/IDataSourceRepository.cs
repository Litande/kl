using KL.Manager.API.Persistent.Entities;

namespace KL.Manager.API.Persistent.Repositories.Interfaces;

public interface IDataSourceRepository
{
    Task<IReadOnlyCollection<DataSource>> GetAllDataSource(CancellationToken ct = default);
}