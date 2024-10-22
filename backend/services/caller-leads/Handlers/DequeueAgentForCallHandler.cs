using KL.Caller.Leads.Handlers.Contracts;
using KL.Caller.Leads.Models.Messages;
using KL.Caller.Leads.Repositories;

namespace KL.Caller.Leads.Handlers;

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
