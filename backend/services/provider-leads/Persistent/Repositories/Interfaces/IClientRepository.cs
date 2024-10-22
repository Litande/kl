using KL.Provider.Leads.Persistent.Entities;

namespace KL.Provider.Leads.Persistent.Repositories.Interfaces;

public interface IClientRepository
{
    Task<IReadOnlyCollection<Client>> GetClients(CancellationToken ct = default);
}