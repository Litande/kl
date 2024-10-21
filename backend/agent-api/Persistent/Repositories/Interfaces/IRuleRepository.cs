using Plat4Me.DialAgentApi.Application.Enums;

namespace Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

public interface IRuleRepository
{
    Task<IReadOnlyCollection<LeadStatusTypes>> GetAvailableStatuses(long clientId, LeadStatusTypes currentStatus, CancellationToken ct = default);
}
