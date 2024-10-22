using KL.Caller.Leads.Models;
using KL.Caller.Leads.Models.Entities;

namespace KL.Caller.Leads.Extensions;

public static class SipProviderExtension
{
    public static SipProviderInfo MapToInfo(this SipProvider provider) => new(
        provider.Id,
        provider.ProviderName,
        provider.ProviderAddress,
        provider.ProviderUserName,
        provider.Status
    );
}
