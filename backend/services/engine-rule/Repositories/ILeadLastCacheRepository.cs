using KL.Engine.Rule.Models;

namespace KL.Engine.Rule.Repositories;

public interface ILeadLastCacheRepository
{
    Task<LeadTrackingCache?> GetLead(long leadId);
    Task<ICollection<LeadTrackingCache>> GetLeads(IEnumerable<long> leads);
    Task UpdateScore(long leadId, long? score);
    Task UpdateLeads(IDictionary<long, long> leadsScores);
}
