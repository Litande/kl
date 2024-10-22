using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Persistent.Repositories.Interfaces;

public interface ISettingsRepository
{
    Task<string?> GetValue(SettingTypes type, long clientId, CancellationToken ct = default);
    Task SetValue(SettingTypes type, long clientId, string value, CancellationToken ct = default);
}