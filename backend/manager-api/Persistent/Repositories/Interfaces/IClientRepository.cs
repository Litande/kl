using KL.Manager.API.Persistent.Entities;

namespace KL.Manager.API.Persistent.Repositories.Interfaces;

public interface IClientRepository
{
    Task<IEnumerable<Client>> GetAll(CancellationToken ct = default);
}
