using KL.Statistics.Application.Models.Responses;

namespace KL.Statistics.Application.Handlers.Dashboard;

public interface IGetAgentsWorkModeHandler
{
    Task<AgentsWorkMode> Handle(long clientId, CancellationToken ct = default);
}