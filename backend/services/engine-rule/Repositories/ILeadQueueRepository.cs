using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Models.Entities;

namespace Plat4Me.DialRuleEngine.Application.Repositories;

public interface ILeadQueueRepository
{
    IReadOnlyCollection<LeadQueue> GetAll();
    Task<IReadOnlyCollection<LeadQueue>> GetAllByClient(long clientId, LeadQueueStatusTypes? status = LeadQueueStatusTypes.Enable, CancellationToken ct = default);
}
