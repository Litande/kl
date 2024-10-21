using Plat4Me.DialAgentApi.Application.Models.Messages;

namespace Plat4Me.DialAgentApi.Application.Handlers;

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
