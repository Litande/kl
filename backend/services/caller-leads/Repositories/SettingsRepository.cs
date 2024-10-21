using Microsoft.EntityFrameworkCore;
using Plat4Me.DialLeadCaller.Application.Enums;
using Plat4Me.DialLeadCaller.Application.Models.Entities;
using Plat4Me.DialLeadCaller.Application.Repositories;

namespace Plat4Me.DialLeadCaller.Infrastructure.Repositories;

public class SettingsRepository : ISettingsRepository
{
    private readonly DialDbContext _context;

    public SettingsRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetValue(
        SettingTypes type,
        long clientId,
        CancellationToken ct = default)
    {
        var value = await _context.Settings
            .Where(x => x.Type == type
                        && x.ClientId == clientId)
            .Select(x => x.Value)
            .FirstOrDefaultAsync(ct);

        return value;
    }

    public async Task<Dictionary<long, string>> GetValuesByClientId(
        SettingTypes type,
        IEnumerable<long> clientIds,
        CancellationToken ct = default)
    {
        var values = await _context.Settings
            .Where(x => x.Type == type
                        && clientIds.Contains(x.ClientId))
            .ToDictionaryAsync(x => x.ClientId, x => x.Value, ct);

        return values;
    }

    public async Task<IDictionary<long, string>> GetValuesForAllClients(
        SettingTypes type,
        CancellationToken ct = default)
    {
        var values = await _context.Settings
            .Where(x => x.Type == type)
            .ToDictionaryAsync(x => x.ClientId, x => x.Value, ct);

        return values;
    }

    public async Task SetValue(
        SettingTypes type,
        long clientId,
        string value,
        CancellationToken ct = default)
    {
        var entity = await _context.Settings
            .Where(x => x.Type == type
                        && x.ClientId == clientId)
            .FirstOrDefaultAsync(ct);

        if (entity is null)
        {
            entity = new SettingsEntity
            {
                Type = type,
                ClientId = clientId
            };
            await _context.Settings.AddAsync(entity, ct);
        }

        entity.Value = value;
        await _context.SaveChangesAsync(ct);
    }
}
