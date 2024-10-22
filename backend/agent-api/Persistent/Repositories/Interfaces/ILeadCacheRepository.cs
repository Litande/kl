using KL.Agent.API.Persistent.Entities.Cache;

namespace KL.Agent.API.Persistent.Repositories.Interfaces;

public interface ILeadCacheRepository
{
    Task<LeadTrackingCache?> GetLead(long leadId);
    Task<Dictionary<long, LeadTrackingCache>> GetLeads(IEnumerable<long> leadIds);
}
