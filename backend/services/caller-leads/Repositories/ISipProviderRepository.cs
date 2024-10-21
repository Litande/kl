using Plat4Me.DialLeadCaller.Application.Models.Entities;

namespace Plat4Me.DialLeadCaller.Application.Repositories;

public interface ISipProviderRepository
{
    Task<List<SipProvider>> GetProviders(CancellationToken ct = default);
    Task<SipProvider?> GetProvider(CancellationToken ct = default);
    Task<SipProvider?> GetProviderById(long providerId, CancellationToken ct = default);
}
