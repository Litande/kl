using Plat4Me.DialAgentApi.Application.Enums;
using Plat4Me.DialAgentApi.Persistent.Entities;

namespace Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

public interface IAgentStatusHistoryRepository
{
    Task AddStatusHistory(AgentStatusHistory statusHistory, CancellationToken ct = default);
}
