using System.Text.Json;
using System.Text.Json.Nodes;
using KL.Agent.API.Application.Enums;
using KL.Agent.API.Persistent.Entities;
using KL.Agent.API.Persistent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KL.Agent.API.Persistent.Repositories;

public class SettingsRepository : ISettingsRepository
{
    private readonly DialDbContext _context;

    public SettingsRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetValue(SettingTypes type, long clientId, CancellationToken ct = default)
    {
        var value = await _context.Settings
            .Where(x => x.Type == type && x.ClientId == clientId)
            .Select(x => x.Value)
            .FirstOrDefaultAsync(ct);
        return value;
    }

    public async Task<Dictionary<string, JsonNode?>?> GetSettings(long clientId, IReadOnlySet<string> settingsSet,
        CancellationToken ct = default)
    {
        if (!settingsSet.Any())
            return null;

        var settingsType = settingsSet.Select(x =>
        {
            var enumType = x.Split('.')[0];
            return (SettingTypes)Enum.Parse(typeof(SettingTypes), enumType);
        }).ToHashSet();

        var settings = await _context.Settings
            .AsNoTracking()
            .Where(x => x.ClientId == clientId
                        && settingsType.Contains(x.Type))
            .ToDictionaryAsync(x => x.Type, x => x.Value, ct);

        return MapSettingsToDictionary(settings, settingsSet);
    }

    private static Dictionary<string, JsonNode?> MapSettingsToDictionary(IDictionary<SettingTypes, string> settings,
        IReadOnlySet<string> keys)
    {
        var keyAndValues = settings
            .SelectMany(x => JsonSerializer.Deserialize<JsonObject>(x.Value)!
                .Select(y => new KeyValuePair<SettingTypes, KeyValuePair<string, JsonNode?>>(x.Key, y)))
            .Where(x => keys.Contains($"{x.Key}.{x.Value.Key}"))
            .ToDictionary(x => x.Value.Key, x => x.Value.Value);

        return keyAndValues;
    }

    public async Task SetValue(SettingTypes type, long clientId, string value, CancellationToken ct = default)
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
