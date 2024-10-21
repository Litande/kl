using Microsoft.EntityFrameworkCore;
using Plat4Me.DialLeadProvider.Application.Enums;
using Plat4Me.DialLeadProvider.Persistent.Entities;
using Plat4Me.DialLeadProvider.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialLeadProvider.Persistent.Repositories;

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