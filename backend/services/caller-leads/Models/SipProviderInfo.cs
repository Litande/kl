using KL.Caller.Leads.Enums;

namespace KL.Caller.Leads.Models;

public record SipProviderInfo(
    long Id, 
    string ProviderName, 
    string ProviderAddress,
    string ProviderUserName,
    SipProviderStatus Status
);