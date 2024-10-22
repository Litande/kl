using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models.Requests.Agents;

public record AgentsFilterRequest(
    IReadOnlyCollection<long>? AgentIds = null,
    IReadOnlyCollection<long>? TagIds = null,
    IReadOnlyCollection<AgentStatusTypes>? AgentStatuses = null,
    IReadOnlyCollection<long>? GroupIds = null);