using System.Text.Json.Nodes;
using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Persistent.Repositories.Interfaces;

public interface ISettingsRepository
{
    Task<string?> GetValue(SettingTypes type, long clientId, CancellationToken ct = default);
    Task<Dictionary<string, JsonNode?>?> GetSettings(long clientId, IReadOnlySet<string> settingsSet, CancellationToken ct = default);
    Task SetValue(SettingTypes type, long clientId, string value, CancellationToken ct = default);
}