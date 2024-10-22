using KL.Agent.API.Application.Models.Messages;

namespace KL.Agent.API.Application.Handlers;

public interface ICallMessagesHandler : 
    ISubHandler<InviteAgentMessage>,
    ISubHandler<CallFinishedMessage>,
    ISubHandler<CalleeAnsweredMessage>,
    ISubHandler<AgentReplacedMessage>,
    ISubHandler<DroppedAgentMessage>,
    ISubHandler<CallInitiatedMessage>,
    ISubHandler<LeadFeedbackFilledMessage>
{
}
