using KL.Provider.Leads.Application.Enums;
using KL.Provider.Leads.Persistent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KL.Provider.Leads.Persistent.Repositories;

public class SettingsRepository : ISettingsRepository
{
    private readonly KlDbContext _context;

    public SettingsRepository(KlDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetValue(SettingTypes type, long clientId, CancellationToken ct = default)
    {
        var value = await _context.Settings
            .Where(x => x.Type == type
                        && x.ClientId == clientId)
            .Select(x => x.Value)
            .FirstOrDefaultAsync(ct);
        return value;
    }
}