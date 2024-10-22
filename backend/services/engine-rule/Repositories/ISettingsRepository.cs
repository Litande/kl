using KL.Engine.Rule.Enums;

namespace KL.Engine.Rule.Repositories;

public interface ISettingsRepository
{
    Task<string?> GetValue(long clientId, SettingTypes type, CancellationToken ct = default);
}
