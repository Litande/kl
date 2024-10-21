using Microsoft.EntityFrameworkCore;
using Plat4Me.DialLeadProvider.Application.Enums;
using Plat4Me.DialLeadProvider.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialLeadProvider.Persistent.Repositories;

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
            .Where(x => x.Type == type
                        && x.ClientId == clientId)
            .Select(x => x.Value)
            .FirstOrDefaultAsync(ct);
        return value;
    }
}