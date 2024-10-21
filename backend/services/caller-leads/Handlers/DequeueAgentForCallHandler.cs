using Plat4Me.DialLeadCaller.Application.Handlers.Contracts;
using Plat4Me.DialLeadCaller.Application.Models.Messages;
using Plat4Me.DialLeadCaller.Application.Repositories;

namespace Plat4Me.DialLeadCaller.Application.Handlers;

public class DequeueAgentForCallHandler : IDequeueAgentForCallHandler
{
    private readonly IAgentStateRepository _agentState;

    public DequeueAgentForCallHandler(
        IAgentStateRepository agentState
    )
    {
        _agentState = agentState;
    }

    public async Task Process(DequeueAgentForCallMessage message, CancellationToken ct = default)
    {
        var agent = await _agentState.GetWaitingAgentById(message.ClientId, message.AgentId, ct);
        if (agent is not null)
            await _agentState.RemoveAgent(agent);
    }

}
