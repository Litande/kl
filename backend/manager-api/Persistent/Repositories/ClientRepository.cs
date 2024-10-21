using Microsoft.EntityFrameworkCore;
using Plat4Me.DialClientApi.Persistent.Entities;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialClientApi.Persistent.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly DialDbContext _context;

    public ClientRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Client>> GetAll(CancellationToken ct = default)
    {
        var entities = await _context.Clients
            .ToArrayAsync(ct);

        return entities;
    }
}
