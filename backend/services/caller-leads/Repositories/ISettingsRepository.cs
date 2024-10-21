using Plat4Me.DialLeadCaller.Application.Enums;

namespace Plat4Me.DialLeadCaller.Application.Repositories;

public interface ISettingsRepository
{
    Task<string?> GetValue(SettingTypes type, long clientId, CancellationToken ct = default);
    Task<Dictionary<long, string>> GetValuesByClientId(SettingTypes type, IEnumerable<long> clientIds, CancellationToken ct = default);
    Task SetValue(SettingTypes type, long clientId, string value, CancellationToken ct = default);
    Task<IDictionary<long, string>> GetValuesForAllClients(SettingTypes type, CancellationToken ct = default);
}
