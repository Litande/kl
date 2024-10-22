using KL.Provider.Leads.Application.Enums;

namespace KL.Provider.Leads.Persistent.Repositories.Interfaces;

public interface ISettingsRepository
{
    Task<string?> GetValue(SettingTypes type, long clientId, CancellationToken ct = default);
}