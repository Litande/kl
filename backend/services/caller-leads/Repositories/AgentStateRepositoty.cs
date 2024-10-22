using KL.Caller.Leads.Models;
using Microsoft.EntityFrameworkCore;
using Redis.OM;
using Redis.OM.Searching;

namespace KL.Caller.Leads.Repositories;

public class AgentStateRepository : IAgentStateRepository
{
    private readonly RedisCollection<WaitingAgent> _agentStates;

    public AgentStateRepository(RedisConnectionProvider redisProvider)
    {
        _agentStates = (RedisCollection<WaitingAgent>)redisProvider.RedisCollection<WaitingAgent>(saveState: false);
    }

    public async Task<IReadOnlyCollection<WaitingAgent>> GetWaitingAgentsForClient(
        long clientId,
        CancellationToken ct = default)
    {
        return await Queryable.Where(_agentStates, r => r.ClientId == clientId)
            .ToArrayAsync(ct);         
    }

    public async Task<WaitingAgent?> GetWaitingAgentById(long clientId, long agentId, CancellationToken ct = default)
    {
        return await Queryable.Where(_agentStates, r => r.ClientId == clientId)
            .FirstOrDefaultAsync(i => i.AgentId == agentId);
    }

    public async Task UpdateAgents(IEnumerable<WaitingAgent> agents)
    {
        await _agentStates.UpdateAsync(agents);
    }

    public async Task UpdateAgent(WaitingAgent agent)
    {
        await _agentStates.UpdateAsync(agent);
    }

    public async Task RemoveAgents(IEnumerable<WaitingAgent> agents, CancellationToken ct = default)
    {
        await _agentStates.DeleteAsync(agents);
    }

    public async Task RemoveAgent(WaitingAgent agent, CancellationToken ct = default)
    {
        await _agentStates.DeleteAsync(agent);
    }

    public async Task RemoveAgent(long id, CancellationToken ct = default)
    {
        var agent = await _agentStates.FindByIdAsync(id.ToString());
        if (agent is not null)
            await _agentStates.DeleteAsync(agent);
    }
}
