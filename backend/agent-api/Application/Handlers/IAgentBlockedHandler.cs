using KL.Agent.API.Application.Models.Messages;

namespace KL.Agent.API.Application.Handlers;


public interface IAgentBlockedHandler : ISubHandler<AgentBlockedMessage>
{
}