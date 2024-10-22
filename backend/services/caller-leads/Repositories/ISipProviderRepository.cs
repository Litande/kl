using KL.Caller.Leads.Models.Entities;

namespace KL.Caller.Leads.Repositories;

public interface ISipProviderRepository
{
    Task<List<SipProvider>> GetProviders(CancellationToken ct = default);
    Task<SipProvider?> GetProvider(CancellationToken ct = default);
    Task<SipProvider?> GetProviderById(long providerId, CancellationToken ct = default);
}
