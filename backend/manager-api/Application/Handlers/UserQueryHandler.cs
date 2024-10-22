using KL.Manager.API.Application.Common;
using KL.Manager.API.Application.Enums;
using KL.Manager.API.Application.Extensions;
using KL.Manager.API.Application.Models.Responses;
using KL.Manager.API.Persistent.Entities.Settings;
using KL.Manager.API.Persistent.Repositories.Interfaces;

namespace KL.Manager.API.Application.Handlers;

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
