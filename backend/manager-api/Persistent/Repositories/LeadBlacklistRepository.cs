using KL.Manager.API.Application.Models.Requests;
using KL.Manager.API.Application.Models.Responses;
using KL.Manager.API.Persistent.Entities;
using KL.Manager.API.Persistent.Entities.Projections;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KL.Manager.API.Persistent.Repositories;

public class LeadBlacklistRepository : RepositoryBase, ILeadBlacklistRepository
{
    private readonly DialDbContext _context;

    public LeadBlacklistRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<LeadBlacklistProjection>> GetAll(
        long clientId,
        PaginationRequest pagination,
        CancellationToken ct = default)
    {
        var q = _context.LeadBlacklists
            .Where(r => r.ClientId == clientId)
            .Include(x => x.Lead)
            .ThenInclude(x => x.DataSource);

        var rq = q.Select(r => new LeadBlacklistProjection
        {
            Id = r.LeadId,
            Phone = r.Lead.Phone,
            Country = r.Lead.CountryCode,
            FirstName = r.Lead.FirstName,
            LastName = r.Lead.LastName,
            Source = r.Lead.DataSource.Name,
            Email = r.Lead.Email,
            LeadStatus = r.Lead.Status,
            Language = r.Lead.LanguageCode,
            LastTimeOnline = r.Lead.LastTimeOnline,
            RegistrationTime = r.Lead.RegistrationTime,
            FirstDepositTime = r.Lead.FirstDepositTime
        });

        return await CreatePaginatedResponse(rq, pagination, ct);
    }

    public async Task Create(
        long clientId,
        long userId,
        long leadId,
        CancellationToken ct = default)
    {
        var isLeadExists = await _context.Leads
            .AnyAsync(x => x.ClientId == clientId
                           && x.Id == leadId, ct);

        if (!isLeadExists)
            throw new KeyNotFoundException($"Cannot find lead with id: {leadId}");

        var entity = new LeadBlacklist
        {
            LeadId = leadId,
            CreatedByUserId = userId,
            ClientId = clientId,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        _context.LeadBlacklists.Add(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> Delete(
        long clientId,
        IEnumerable<long> leadIds,
        CancellationToken ct = default)
    {
        var entities = await _context.LeadBlacklists
            .Where(x => x.ClientId == clientId
                        && leadIds.Contains(x.LeadId))
            .ToListAsync(ct);

        if (!entities.Any()) return false;

        _context.RemoveRange(entities);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}
