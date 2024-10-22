using KL.SIP.Bridge.Application.Enums;

namespace KL.SIP.Bridge.Application.Models;

public record SipProviderInfo(
    long Id,
    string ProviderName,
    string ProviderAddress,
    string ProviderUserName,
    SipProviderStatus Status
);