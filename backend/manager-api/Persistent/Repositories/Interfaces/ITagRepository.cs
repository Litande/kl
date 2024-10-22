using KL.Manager.API.Application.Enums;
using KL.Manager.API.Application.Models.Requests.Tags;
using KL.Manager.API.Application.Models.Responses.Tags;

namespace KL.Manager.API.Persistent.Repositories.Interfaces;

public interface ITagRepository
{
    Task<IEnumerable<TagResponse>> GetAll(long clientId, TagStatusTypes? filterByStatus = null, CancellationToken ct = default);
    Task<TagResponse?> GetById(long clientId, int tagId, CancellationToken ct = default);
    Task<TagResponse> Create(long clientId, CreateTagRequest request, CancellationToken ct = default);
    Task<TagResponse?> Update(long clientId, int tagId, UpdateTagRequest request, CancellationToken ct = default);
    Task<bool> Delete(long clientId, int tagId, CancellationToken ct = default);
}