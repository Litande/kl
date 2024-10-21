using Plat4Me.DialRuleEngine.Application.Enums;

namespace Plat4Me.DialRuleEngine.Application.Repositories;

public interface ISettingsRepository
{
    Task<string?> GetValue(long clientId, SettingTypes type, CancellationToken ct = default);
}
