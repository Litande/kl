using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Plat4Me.DialClientApi.Application.Models.Requests.UserFilter;
using Plat4Me.DialClientApi.Application.Models.Responses.UserFilter;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialClientApi.Controllers;

[Route("filters")]
public class FiltersController : ApiAuthorizeBase
{
    private readonly IUserFilterPreferencesRepository _userFilterRepository;
    private readonly IDataSourceRepository _dataSourceRepository;

    public FiltersController(
        IUserFilterPreferencesRepository userFilterRepository, 
        IDataSourceRepository dataSourceRepository)
    {
        _userFilterRepository = userFilterRepository;
        _dataSourceRepository = dataSourceRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserFilters(CancellationToken ct = default)
    {
        var filters = await _userFilterRepository.GetUserFilters(CurrentUserId, ct);
        return Ok(filters);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFilter(
        [FromBody] CreateUserFilterRequest request,
        CancellationToken ct = default)
    {
        await _userFilterRepository.AddFilter(CurrentUserId, request, ct);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateFilter(
        [FromBody] UpdateUserFilterRequest request,
        CancellationToken ct = default)
    {
        await _userFilterRepository.UpdateFilter(CurrentUserId, request, ct);
        return Ok();
    }

    [HttpDelete("{filterId:long}")]
    public async Task<IActionResult> DeleteFilter(
        [FromRoute] long filterId,
        CancellationToken ct = default)
    {
        await _userFilterRepository.DeleteFilter(CurrentUserId, filterId, ct);
        return Ok();
    }

    [HttpGet("leadSearch")]
    public async Task<IActionResult> GetLeadSearchFilters(CancellationToken ct = default)
    {
        var dataSources = await _dataSourceRepository.GetAllDataSource(ct);
        var filters = dataSources.Where(x=>x.Metadata is not null)
            .Select(x => JsonSerializer.Deserialize<FilterResponse>(x.Metadata!)!);
        return Ok(filters);
    }
}