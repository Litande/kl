using KL.Agent.API.Persistent.Entities;

namespace KL.Agent.API.Persistent.Repositories.Interfaces;

public interface IAgentStatusHistoryRepository
{
    Task AddStatusHistory(AgentStatusHistory statusHistory, CancellationToken ct = default);
}
