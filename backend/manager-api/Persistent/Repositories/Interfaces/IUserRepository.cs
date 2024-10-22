using KL.Manager.API.Application.Models.Requests;
using KL.Manager.API.Application.Models.Requests.Agents;
using KL.Manager.API.Application.Models.Responses;
using KL.Manager.API.Application.Models.Responses.Agents;
using KL.Manager.API.Persistent.Entities;
using KL.Manager.API.Persistent.Entities.Projections;

namespace KL.Manager.API.Persistent.Repositories.Interfaces;

public interface IUserRepository
{
    Task<AgentsResponse> GetByTeams(long clientId, AgentsFilterRequest? filters, CancellationToken ct = default);
    Task<User?> GetById(long clientId, long userId, CancellationToken ct = default);
    Task<AgentProjection?> GetWithTeamsTagsAndQueues(long clientId, long agentId, CancellationToken ct = default);
    Task<AgentProjection?> Create(long clientId, long currentUserId, CreateAgentRequest request, CancellationToken ct = default);
    Task<AgentProjection?> Update(long clientId, long agentId, UpdateAgentRequest request, CancellationToken ct = default);
    Task<PaginatedResponse<AgentTagsProjection>> GetByTagIds(long clientId, PaginationRequest pagination, IReadOnlyCollection<long>? tagIds = null, CancellationToken ct = default);
    Task<IEnumerable<TagProjection>> UpdateTags(long clientId, long agentId, IReadOnlyCollection<long>? tagIds, CancellationToken ct = default);
    Task<AgentInfoProjection?> GetAgentInfoById(long clientId, long agentId, CancellationToken ct = default);
    Task<IReadOnlyCollection<AgentInfoProjection>> GetAgentInfoByIds(long clientId, IEnumerable<long> agentIds, CancellationToken ct = default);
    Task<IReadOnlyCollection<AgentInfoProjection>> GetAgentInfoByClientId(long clientId, CancellationToken ct = default);
}
