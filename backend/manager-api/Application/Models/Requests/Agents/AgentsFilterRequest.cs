using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Models.Requests.Agents;

public record AgentsFilterRequest(
    IReadOnlyCollection<long>? AgentIds = null,
    IReadOnlyCollection<long>? TagIds = null,
    IReadOnlyCollection<AgentStatusTypes>? AgentStatuses = null,
    IReadOnlyCollection<long>? GroupIds = null);