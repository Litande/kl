using Plat4Me.DialLeadCaller.Application.Models.Entities;

namespace Plat4Me.DialLeadCaller.Application.Services.Contracts;

public interface ISipProviderService
{
    Task<SipProvider?> GetProviderForPredictiveCall(CancellationToken ct = default);
    Task<SipProvider?> GetProviderForManualCall(CancellationToken ct = default);
    Task<SipProvider?> GetProviderForRecall(long originalProviderId, CancellationToken ct = default);
}
