using KL.Caller.Leads.Models.Entities.Settings;

namespace KL.Caller.Leads.Services;

public interface ISettingsService
{
    Task<ProductiveDialerSettings> GetProductiveDialerSettingsOrDefault(long clientId, CancellationToken ct = default);
    Task<IDictionary<long, ProductiveDialerSettings>> GetProductiveDialerSettingsOrDefault(IReadOnlyCollection<long> clientIds, CancellationToken ct = default);
    Task<RtcConfigurationSettings?> GetRtcSettings(long clientId, CancellationToken ct = default);
    Task<IDictionary<long, RtcConfigurationSettings?>> GetRtcSettings(IReadOnlyCollection<long> clientIds, CancellationToken ct = default);
}
