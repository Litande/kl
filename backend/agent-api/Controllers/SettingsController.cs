using Microsoft.AspNetCore.Mvc;
using Plat4Me.DialAgentApi.Persistent.Configurations;
using Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialAgentApi.Controllers;

[Route("settings")]
public class SettingsController : ApiAuthorizeBase
{
    private readonly ISettingsRepository _settingsRepository;

    public SettingsController(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetSettings(CancellationToken ct = default)
    {
        var settings = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            SettingConstants.ProductiveDialer.EndCallButtonAfter,
            SettingConstants.ProductiveDialer.MaxCallDuration,
            SettingConstants.VoiceMail.ShowVoicemailButton,
            SettingConstants.VoiceMail.HideVoicemailButton,
            SettingConstants.Feedback.PageTimeout,
            SettingConstants.Feedback.RedialsLimit,
        };
        var keyAndValues = await _settingsRepository.GetSettings(CurrentClientId, settings, ct);

        if (keyAndValues is null)
            return NotFound();

        return Ok(keyAndValues);
    }
}
