using Plat4Me.Dial.Statistic.Api.Application.Models.Responses;

namespace Plat4Me.Dial.Statistic.Api.Application.Handlers.Dashboard;

public interface IGetAgentsWorkModeHandler
{
    Task<AgentsWorkMode> Handle(long clientId, CancellationToken ct = default);
}