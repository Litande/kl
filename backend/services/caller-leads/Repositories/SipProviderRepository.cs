﻿using KL.Caller.Leads.Enums;
using KL.Caller.Leads.Models.Entities;

namespace KL.Caller.Leads.Repositories;

public class SipProviderRepository : ISipProviderRepository
{
    private readonly DialDbContext _context;

    public SipProviderRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<List<SipProvider>> GetProviders(CancellationToken ct = default)
    {
        return await _context.SipProviders
            .Where(x => x.Status == SipProviderStatus.Enable)
            .ToListAsync(ct);
    }

    public async Task<SipProvider?> GetProvider(CancellationToken ct = default)
    {
        return await _context.SipProviders
            .FirstOrDefaultAsync(x => x.Status == SipProviderStatus.Enable, ct);
    }

    public async Task<SipProvider?> GetProviderById(long providerId, CancellationToken ct = default)
    {
        return await _context.SipProviders
           .FirstOrDefaultAsync(x => x.Status == SipProviderStatus.Enable && x.Id == providerId, ct);
    }
}