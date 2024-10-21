using Plat4Me.DialLeadCaller.Application.Enums;

namespace Plat4Me.DialLeadCaller.Application.Models;

public record SipProviderInfo(
    long Id, 
    string ProviderName, 
    string ProviderAddress,
    string ProviderUserName,
    SipProviderStatus Status
);