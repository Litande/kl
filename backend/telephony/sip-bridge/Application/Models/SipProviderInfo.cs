using Plat4Me.DialSipBridge.Application.Enums;

namespace Plat4Me.DialSipBridge.Application.Models;

public record SipProviderInfo(
    long Id,
    string ProviderName,
    string ProviderAddress,
    string ProviderUserName,
    SipProviderStatus Status
);