using Plat4Me.Dial.Statistic.Api.Application.Models.Messages;

namespace Plat4Me.Dial.Statistic.Api.Application.Handlers.Agent;

public interface IAgentStatisticsChangeHandler : ISubHandler<AgentsChangedStatusMessage>
{
}