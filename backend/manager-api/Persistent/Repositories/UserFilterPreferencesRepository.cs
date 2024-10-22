using System.Text.Json;
using KL.Manager.API.Application.Models;
using KL.Manager.API.Application.Models.Requests.UserFilter;
using KL.Manager.API.Application.Models.Responses.UserFilter;
using KL.Manager.API.Persistent.Entities;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KL.Manager.API.Persistent.Repositories;

public class UserFilterPreferencesRepository : IUserFilterPreferencesRepository
{
    private readonly KlDbContext _context;

    public UserFilterPreferencesRepository(KlDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<UserFilterResponse>> GetUserFilters(long userId, CancellationToken ct = default)
    {
        var filters =  await _context.UserFilterPreferences
            .Where(x => x.CreatedById == userId)
            .ToArrayAsync(ct);

        return filters.Select(x =>
        {
            var filter = JsonSerializer.Deserialize<LeadSearchGroupParams[]>(x.Filter);
            return new UserFilterResponse
            {
                Id = x.Id,
                FilterName = x.FilterName,
                Filter = filter!,
            };
        }).ToArray();
    }

    public async Task AddFilter(
        long userId, CreateUserFilterRequest request,
        CancellationToken ct = default)
    {
        var userFilterPreferences = new UserFilterPreferences
        {
            CreatedById = userId,
            FilterName = request.FilterName,
            Filter = JsonSerializer.Serialize(request.Filter),
        };

        await _context.AddAsync(userFilterPreferences, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateFilter(
        long userId, UpdateUserFilterRequest request,
        CancellationToken ct = default)
    {
        var userFilterPreferences = new UserFilterPreferences
        {
            CreatedById = userId,
            FilterName = request.FilterName,
            Filter = JsonSerializer.Serialize(request.Filter),
        };

        _context.Update(userFilterPreferences);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteFilter(long userId, long filterId, CancellationToken ct = default)
    {
        var filter = await _context.UserFilterPreferences
            .FirstOrDefaultAsync(x => x.CreatedById == userId && x.Id == filterId, ct);

        if (filter is not null)
        {
            _context.UserFilterPreferences.Remove(filter);
            await _context.SaveChangesAsync(ct);
        }
    }
}