﻿using KL.Engine.Rule.Enums;
using Microsoft.EntityFrameworkCore;

namespace KL.Engine.Rule.Repositories;

public class SettingsRepository : ISettingsRepository
{
    private readonly KlDbContext _context;

    public SettingsRepository(KlDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetValue(long clientId, SettingTypes type, CancellationToken ct = default)
    {
        var value = await _context.Settings
            .Where(x => x.Type == type
                        && x.ClientId == clientId)
            .Select(x => x.Value)
            .FirstOrDefaultAsync(ct);

        return value;
    }
}
