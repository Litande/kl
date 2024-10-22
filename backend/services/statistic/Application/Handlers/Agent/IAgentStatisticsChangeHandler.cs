using KL.Statistics.Application.Models.Messages;

namespace KL.Statistics.Application.Handlers.Agent;

public interface IAgentStatisticsChangeHandler : ISubHandler<AgentsChangedStatusMessage>
{
}