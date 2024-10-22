using KL.Manager.API.Persistent.Entities.Cache;

namespace KL.Manager.API.Persistent.Repositories.Interfaces;

public interface ILeadLastCacheRepository
{
    Task<LeadTrackingCache?> GetLead(long leadId);
    Task<Dictionary<long,LeadTrackingCache>> GetLeads(IEnumerable<long> leadIds);
}
