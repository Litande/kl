using Microsoft.Extensions.Logging;
using Plat4Me.DialAgentApi.Application.Common;
using Plat4Me.DialAgentApi.Application.Enums;
using Plat4Me.DialAgentApi.Application.Extensions;
using Plat4Me.DialAgentApi.Application.Models.Responses;
using Plat4Me.DialAgentApi.Persistent.Entities.Settings;
using Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialAgentApi.Application.Handlers;

public class UserQueryHandler : IUserQueryHandler
{
    private readonly IUserRepository _userRepository;
    private readonly ISettingsRepository _settingsRepository;
    private readonly ILogger<UserQueryHandler> _logger;

    public UserQueryHandler(
        IUserRepository userRepository,
        ISettingsRepository settingsRepository,
        ILogger<UserQueryHandler> logger)
    {
        _userRepository = userRepository;
        _settingsRepository = settingsRepository;
        _logger = logger;
    }

    public async Task<MeResponse?> Handle(long clientId, long userId, CancellationToken ct = default)
    {
        var user = await _userRepository.GetById(clientId, userId, ct);
        var jsonValue = await _settingsRepository.GetValue(SettingTypes.RTCConfiguration, clientId, ct);
        var setting = JsonHelper.Deserialize<RTCConfigurationSettings>(jsonValue, clientId, _logger);

        return user?.ToResponse(setting?.IceServers);
    }
}