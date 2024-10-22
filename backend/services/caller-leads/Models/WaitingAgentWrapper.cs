namespace KL.Caller.Leads.Models;

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
