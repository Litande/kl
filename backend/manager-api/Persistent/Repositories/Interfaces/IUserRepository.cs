using Plat4Me.DialClientApi.Application.Models.Requests;
using Plat4Me.DialClientApi.Application.Models.Requests.Agents;
using Plat4Me.DialClientApi.Application.Models.Responses;
using Plat4Me.DialClientApi.Application.Models.Responses.Agents;
using Plat4Me.DialClientApi.Persistent.Entities;
using Plat4Me.DialClientApi.Persistent.Entities.Projections;

namespace Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

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
