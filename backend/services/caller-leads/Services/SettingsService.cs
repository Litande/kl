using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plat4Me.DialLeadCaller.Application.App;
using Plat4Me.DialLeadCaller.Application.Enums;
using Plat4Me.DialLeadCaller.Application.Extensions;
using Plat4Me.DialLeadCaller.Application.Models.Entities.Settings;
using Plat4Me.DialLeadCaller.Application.Repositories;

namespace Plat4Me.DialLeadCaller.Application.Services;

public class SettingsService : ISettingsService
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly DialerOptions _dialerOptions;
    private readonly ILogger<SettingsService> _logger;

    public SettingsService(
        ISettingsRepository settingsRepository,
        IOptions<DialerOptions> dialerOptions,
        ILogger<SettingsService> logger)
    {
        _settingsRepository = settingsRepository;
        _dialerOptions = dialerOptions.Value;
        _logger = logger;
    }

    public async Task<ProductiveDialerSettings> GetProductiveDialerSettingsOrDefault(
        long clientId,
        CancellationToken ct = default)
    {
        var settingsJson = await _settingsRepository
            .GetValue(SettingTypes.ProductiveDialer, clientId, ct);

        return DeserializeSettings<ProductiveDialerSettings>(clientId, settingsJson)
               ?? new ProductiveDialerSettings
        {
            RingingTimeout = _dialerOptions.DefaultRingingTimeout,
            MaxCallDuration = _dialerOptions.DefaultMaxCallDuration
        };
    }

    public async Task<IDictionary<long, ProductiveDialerSettings>> GetProductiveDialerSettingsOrDefault(
        IReadOnlyCollection<long> clientIds,
        CancellationToken ct = default)
    {
        var settingsMap = await _settingsRepository
            .GetValuesByClientId(SettingTypes.ProductiveDialer, clientIds, ct);

        var result = clientIds.ToDictionary(
            clientId => clientId,
            clientId =>
            {
                settingsMap.TryGetValue(clientId, out var settingsJson);
                var settings = DeserializeSettings<ProductiveDialerSettings?>(clientId, settingsJson);
                if (settings is not null)
                    return settings;

                return new ProductiveDialerSettings
                {
                    RingingTimeout = _dialerOptions.DefaultRingingTimeout,
                    MaxCallDuration = _dialerOptions.DefaultMaxCallDuration
                };
            });

        return result;
    }

    public async Task<RtcConfigurationSettings?> GetRtcSettings(
        long clientId,
        CancellationToken ct = default)
    {
        var settingsJson = await _settingsRepository
            .GetValue(SettingTypes.RTCConfiguration, clientId, ct);

        return DeserializeSettings<RtcConfigurationSettings>(clientId, settingsJson);
    }

    public async Task<IDictionary<long, RtcConfigurationSettings?>> GetRtcSettings(
        IReadOnlyCollection<long> clientIds,
        CancellationToken ct = default)
    {
        var settingsMap = await _settingsRepository
            .GetValuesByClientId(SettingTypes.RTCConfiguration, clientIds, ct);

        return clientIds.ToDictionary(
            clientId => clientId,
            clientId =>
            {
                settingsMap.TryGetValue(clientId, out var settingsJson);
                return DeserializeSettings<RtcConfigurationSettings?>(clientId, settingsJson);
            });
    }

    private T? DeserializeSettings<T>(
        long clientId,
        string? value)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _logger.LogWarning("The {setting} JSON value for client {clientId} is empty or null",
                    nameof(T), clientId);

                return default;
            }

            var settings = JsonSerializer.Deserialize<T?>(value, JsonSettingsExtensions.Default);
            if (settings is null)
                _logger.LogError("The {setting} JSON value parsing failed for client {clientId}",
                    nameof(T), clientId);

            return settings;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "The {setting} JSON value parsing failed for client {clientId}",
                nameof(T), clientId);

            return default;
        }
    }
}
