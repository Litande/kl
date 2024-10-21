using Microsoft.EntityFrameworkCore;
using Plat4Me.DialLeadCaller.Application.Models;
using Plat4Me.DialLeadCaller.Application.Repositories;
using Redis.OM;
using Redis.OM.Searching;

namespace Plat4Me.DialLeadCaller.Infrastructure.Repositories;

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
        return await _agentStates
            .Where(r => r.ClientId == clientId)
            .ToArrayAsync(ct);         
    }

    public async Task<WaitingAgent?> GetWaitingAgentById(long clientId, long agentId, CancellationToken ct = default)
    {
        return await _agentStates
            .Where(r => r.ClientId == clientId)
            .FindByIdAsync(agentId.ToString());
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
