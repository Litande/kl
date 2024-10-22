using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models.Entities;

namespace KL.Engine.Rule.Repositories;

public interface ILeadQueueRepository
{
    IReadOnlyCollection<LeadQueue> GetAll();
    Task<IReadOnlyCollection<LeadQueue>> GetAllByClient(long clientId, LeadQueueStatusTypes? status = LeadQueueStatusTypes.Enable, CancellationToken ct = default);
}
