using Plat4Me.DialLeadProvider.Application.Enums;

namespace Plat4Me.DialLeadProvider.Persistent.Repositories.Interfaces;

public interface ISettingsRepository
{
    Task<string?> GetValue(SettingTypes type, long clientId, CancellationToken ct = default);
}