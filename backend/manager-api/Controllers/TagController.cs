using KL.Manager.API.Application.Enums;
using KL.Manager.API.Application.Models.Requests.Tags;
using KL.Manager.API.Application.Models.Responses.Tags;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KL.Manager.API.Controllers;

[Route("tags")]
public class TagController : ApiAuthorizeBase
{
    private readonly ITagRepository _tagRepository;

    public TagController(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] TagStatusTypes? filterByStatus = null,
        CancellationToken ct = default)
    {
        var items = await _tagRepository.GetAll(CurrentClientId, filterByStatus, ct);
        var response = new TagsResponse(items);
        return Ok(response);
    }

    [HttpGet]
    [Route("{tagId}")]
    public async Task<IActionResult> Get(
        [FromRoute] int tagId,
        CancellationToken ct = default)
    {
        var response = await _tagRepository.GetById(CurrentClientId, tagId, ct);
        if (response is null) return NotFound();

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post(
        [FromBody] CreateTagRequest request,
        CancellationToken ct = default)
    {
        var response = await _tagRepository.Create(CurrentClientId, request, ct);
        return Ok(response);
    }

    [HttpPut]
    [Route("{tagId}")]
    public async Task<IActionResult> Put(
        [FromRoute] int tagId,
        [FromBody] UpdateTagRequest request,
        CancellationToken ct = default)
    {
        var response = await _tagRepository.Update(CurrentClientId, tagId, request, ct);
        if (response is null) return NotFound();

        return Ok(response);
    }

    [HttpDelete]
    [Route("{tagId}")]
    public async Task<IActionResult> Delete(
        [FromRoute] int tagId,
        CancellationToken ct = default)
    {
        var response = await _tagRepository.Delete(CurrentClientId, tagId, ct);
        if (!response) return NotFound();

        return Ok();
    }
}
