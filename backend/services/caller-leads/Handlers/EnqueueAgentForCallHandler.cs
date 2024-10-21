using Plat4Me.DialLeadCaller.Application.Handlers.Contracts;
using Plat4Me.DialLeadCaller.Application.Models.Messages;
using Plat4Me.DialLeadCaller.Application.Repositories;
using Plat4Me.DialLeadCaller.Application.Models;

namespace Plat4Me.DialLeadCaller.Application.Handlers;

public class EnqueueAgentForCallHandler : IEnqueueAgentForCallHandler
{
    private readonly IAgentStateRepository _agentState;


    public EnqueueAgentForCallHandler(
        IAgentStateRepository agentState
    )
    {
        _agentState = agentState;
    }

    public async Task Process(EnqueueAgentForCallMessage message, CancellationToken ct = default)
    {
        var agent = new WaitingAgent(message.ClientId, message.AgentId);
        await _agentState.UpdateAgent(agent);
    }
}
