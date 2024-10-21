namespace Plat4Me.DialLeadCaller.Application.Models;

public class WaitingAgentWrapper
{
    public WaitingAgentWrapper(long agentId, int callsCount)
    {
        AgentId = agentId;
        CallsCount = callsCount;
    }

    public long AgentId { get; set; }
    public int CallsCount { get; set; }
}
