using Microsoft.AspNetCore.Mvc;
using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Application.Extensions;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialClientApi.Controllers;

[Route("settings")]
public class SettingsController : ApiAuthorizeBase
{
    private readonly ISettingsRepository _settingsRepository;

    public SettingsController(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    [HttpGet]
    [Route("")]
    public async Task<ActionResult> ListSettings(
        CancellationToken ct = default)
    {
        var result = EnumExtensions.EnumToList<SettingTypes>().Select(x => x.Value);
        return Ok(result);
    }

    [HttpGet]
    [Route("{settingsType}")]
    public async Task<ActionResult> GetSettings(
        [FromRoute] SettingTypes settingsType,
        CancellationToken ct = default)
    {
        var result = await _settingsRepository.GetValue(settingsType, CurrentClientId, ct);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpPut]
    [Route("{settingsType}")]
    public async Task<ActionResult> SaveSettings(
        [FromBody] object request,
        [FromRoute] SettingTypes settingsType,
        CancellationToken ct = default)
    {
        if (request is null)
            return BadRequest();
        await _settingsRepository.SetValue(settingsType, CurrentClientId, request.ToString() ?? "", ct);
        return Ok();
    }
}