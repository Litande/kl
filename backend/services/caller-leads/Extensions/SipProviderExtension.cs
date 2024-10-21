using Plat4Me.DialLeadCaller.Application.Models;
using Plat4Me.DialLeadCaller.Application.Models.Entities;

namespace Plat4Me.DialLeadCaller.Application.Extensions;

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
