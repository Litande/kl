using KL.Manager.API.Application.Models.Responses.AgentTrackings;

namespace KL.Manager.API.Application.Handlers.LiveTracking;

public interface IAgentsQueryHandler
{
    Task<IEnumerable<AgentTrackingResponse>> Handle(long clientId, CancellationToken ct = default);
}
