using Plat4Me.DialClientApi.Persistent.Entities;

namespace Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

public interface IClientRepository
{
    Task<IEnumerable<Client>> GetAll(CancellationToken ct = default);
}
