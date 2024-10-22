using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Persistent.Repositories.Interfaces;

public interface IRuleRepository
{
    Task<IReadOnlyCollection<LeadStatusTypes>> GetAvailableStatuses(long clientId, LeadStatusTypes currentStatus, CancellationToken ct = default);
}
