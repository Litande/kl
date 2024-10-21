using Plat4Me.DialClientApi.Persistent.Entities.Cache;

namespace Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

public interface ILeadLastCacheRepository
{
    Task<LeadTrackingCache?> GetLead(long leadId);
    Task<Dictionary<long,LeadTrackingCache>> GetLeads(IEnumerable<long> leadIds);
}
