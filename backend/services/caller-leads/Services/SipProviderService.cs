using KL.Caller.Leads.Models.Entities;
using KL.Caller.Leads.Repositories;
using KL.Caller.Leads.Services.Contracts;

namespace KL.Caller.Leads.Services;

public class SipProviderService : ISipProviderService
{
    private readonly ILogger<SipProviderService> _logger;
    private readonly ISipProviderRepository _sipProviderRepository;


    public SipProviderService(
        ILogger<SipProviderService> logger,
        ISipProviderRepository sipProviderRepository
    )
    {
        _logger = logger;
        _sipProviderRepository = sipProviderRepository;
    }

    public async Task<SipProvider?> GetProviderForPredictiveCall(CancellationToken ct = default)
    {
        return await _sipProviderRepository.GetProvider(ct);
    }

    public async Task<SipProvider?> GetProviderForManualCall(CancellationToken ct = default)
    {
        return await _sipProviderRepository.GetProvider(ct);
    }

    public async Task<SipProvider?> GetProviderForRecall(long originalProviderId, CancellationToken ct = default)
    {
        var result = await _sipProviderRepository.GetProviderById(originalProviderId, ct);
        if (result is null) //fallback to general //TODO ? add settings for fallback strategy 
            result = await GetProviderForPredictiveCall(ct);
        return result;
    }
}
