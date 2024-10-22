using KL.Caller.Leads.Models.Entities;

namespace KL.Caller.Leads.Services.Contracts;

public interface ISipProviderService
{
    Task<SipProvider?> GetProviderForPredictiveCall(CancellationToken ct = default);
    Task<SipProvider?> GetProviderForManualCall(CancellationToken ct = default);
    Task<SipProvider?> GetProviderForRecall(long originalProviderId, CancellationToken ct = default);
}
