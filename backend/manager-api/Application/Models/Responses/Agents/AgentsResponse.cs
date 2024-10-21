using Plat4Me.DialClientApi.Persistent.Entities.Projections;

namespace Plat4Me.DialClientApi.Application.Models.Responses.Agents;

public record AgentsResponse(IEnumerable<TeamAgentProjection> Items);