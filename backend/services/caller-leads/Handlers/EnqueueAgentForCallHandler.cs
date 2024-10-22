using KL.Caller.Leads.Handlers.Contracts;
using KL.Caller.Leads.Models;
using KL.Caller.Leads.Models.Messages;
using KL.Caller.Leads.Repositories;

namespace KL.Caller.Leads.Handlers;

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
