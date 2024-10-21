using Plat4Me.DialLeadCaller.Application.Models;

namespace Plat4Me.DialLeadCaller.Application.Repositories;

public interface ILeadCacheRepository
{
    Task<LeadTrackingCache?> GetLead(long leadId);
    Task<ICollection<LeadTrackingCache>> GetLeads(IEnumerable<long> leads);
}
