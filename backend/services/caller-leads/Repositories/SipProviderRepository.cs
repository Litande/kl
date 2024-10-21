using Microsoft.EntityFrameworkCore;
using Plat4Me.DialLeadCaller.Application.Enums;
using Plat4Me.DialLeadCaller.Application.Models.Entities;
using Plat4Me.DialLeadCaller.Application.Repositories;

namespace Plat4Me.DialLeadCaller.Infrastructure.Repositories;

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
