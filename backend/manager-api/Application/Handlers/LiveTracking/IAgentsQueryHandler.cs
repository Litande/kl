using Plat4Me.DialClientApi.Application.Models.Responses.AgentTrackings;

namespace Plat4Me.DialClientApi.Application.Handlers.LiveTracking;

public interface IAgentsQueryHandler
{
    Task<IEnumerable<AgentTrackingResponse>> Handle(long clientId, CancellationToken ct = default);
}
