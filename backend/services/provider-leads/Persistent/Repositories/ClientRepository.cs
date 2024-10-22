using KL.Provider.Leads.Application.Enums;
using KL.Provider.Leads.Persistent.Entities;
using KL.Provider.Leads.Persistent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KL.Provider.Leads.Persistent.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly DialDbContext _context;

    public ClientRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Client>> GetClients(CancellationToken ct = default)
    {
        var clients = await _context.Clients
            .Include(i => i.DataSources
                .Where(s => s.Status == DataSourceStatusType.Enable))
            .ToListAsync(ct);

        return clients;
    }
}