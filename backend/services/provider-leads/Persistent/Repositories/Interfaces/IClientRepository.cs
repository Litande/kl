using Plat4Me.DialLeadProvider.Persistent.Entities;

namespace Plat4Me.DialLeadProvider.Persistent.Repositories.Interfaces;

public interface IClientRepository
{
    Task<IReadOnlyCollection<Client>> GetClients(CancellationToken ct = default);
}