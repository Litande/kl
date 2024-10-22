using KL.Agent.API.Application.Common;
using KL.Agent.API.Application.Enums;
using KL.Agent.API.Application.Extensions;
using KL.Agent.API.Application.Models.Responses;
using KL.Agent.API.Persistent.Entities.Settings;
using KL.Agent.API.Persistent.Repositories.Interfaces;

namespace KL.Agent.API.Application.Handlers;

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