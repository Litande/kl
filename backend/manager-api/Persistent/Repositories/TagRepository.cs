using Microsoft.EntityFrameworkCore;
using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Application.Extensions;
using Plat4Me.DialClientApi.Application.Models.Requests.Tags;
using Plat4Me.DialClientApi.Application.Models.Responses.Tags;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialClientApi.Persistent.Repositories;

public class TagRepository : ITagRepository
{
    private readonly DialDbContext _context;

    public TagRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TagResponse>> GetAll(
        long clientId,
        TagStatusTypes? filterByStatus = null,
        CancellationToken ct = default)
    {
        var q = _context.Tags.Where(r => r.ClientId == clientId);

        if (filterByStatus.HasValue)
            q = q.Where(r => r.Status == filterByStatus);

        var response = await q
            .Select(r => r.ToResponse())
            .ToArrayAsync(ct);

        return response;
    }

    public async Task<TagResponse?> GetById(
        long clientId,
        int tagId,
        CancellationToken ct = default)
    {
        var entity = await _context.Tags
            .Where(r => r.ClientId == clientId
                        && r.Id == tagId)
            .FirstOrDefaultAsync(ct);

        return entity?.ToResponse();
    }

    public async Task<TagResponse> Create(
        long clientId,
        CreateTagRequest request,
        CancellationToken ct = default)
    {
        var entity = request.ToModel(clientId);
        _context.Tags.Add(entity);
        await _context.SaveChangesAsync(ct);

        return entity.ToResponse();
    }

    public async Task<TagResponse?> Update(
        long clientId,
        int tagId,
        UpdateTagRequest request,
        CancellationToken ct = default)
    {
        var entity = await _context.Tags
            .Where(r => r.ClientId == clientId
                        && r.Id == tagId)
            .FirstOrDefaultAsync(ct);

        if (entity is null) return null;

        entity.ToModel(request);
        await _context.SaveChangesAsync(ct);

        return entity.ToResponse();
    }

    public async Task<bool> Delete(
        long clientId,
        int tagId,
        CancellationToken ct = default)
    {
        var entity = await _context.Tags
            .Where(r => r.ClientId == clientId
                        && r.Id == tagId)
            .Include(r => r.UserTags)
            .FirstOrDefaultAsync(ct);

        if (entity is null) return false;

        foreach (var item in entity.UserTags)
            entity.UserTags.Remove(item);

        _context.Tags.Remove(entity);
        await _context.SaveChangesAsync(ct);

        return true;
    }
}