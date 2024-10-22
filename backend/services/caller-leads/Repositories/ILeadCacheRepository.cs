using KL.Caller.Leads.Models;

namespace KL.Caller.Leads.Repositories;

public interface ILeadCacheRepository
{
    Task<LeadTrackingCache?> GetLead(long leadId);
    Task<ICollection<LeadTrackingCache>> GetLeads(IEnumerable<long> leads);
}
