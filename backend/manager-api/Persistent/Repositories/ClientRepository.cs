using KL.Manager.API.Persistent.Entities;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KL.Manager.API.Persistent.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly KlDbContext _context;

    public ClientRepository(KlDbContext context)
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
