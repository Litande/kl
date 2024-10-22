using KL.Manager.API.Persistent.Entities.Projections;

namespace KL.Manager.API.Application.Models.Responses.Agents;

public record AgentsResponse(IEnumerable<TeamAgentProjection> Items);