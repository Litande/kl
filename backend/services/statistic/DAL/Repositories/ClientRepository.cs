﻿using KL.Statistics.Application.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KL.Statistics.DAL.Repositories;

public class ClientRepository: IClientRepository
{
    private readonly DialDbContext _context;

    public ClientRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Client>> GetAll(CancellationToken ct = default)
    {
        var entities = await _context.Clients.ToArrayAsync(ct);
        return entities;
    }
}