using System.Text.Json;
using KL.Manager.API.Application.Models.Requests.UserFilter;
using KL.Manager.API.Application.Models.Responses.UserFilter;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KL.Manager.API.Controllers;

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