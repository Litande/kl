using KL.Manager.API.Application.Models.Messages.Agents;

namespace KL.Manager.API.Application.Handlers.LiveTracking;

public interface IAgentChangedStatusHandler : ISubHandler<AgentChangedStatusMessage>
{
}
