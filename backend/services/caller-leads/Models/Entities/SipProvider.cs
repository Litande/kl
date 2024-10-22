using KL.Caller.Leads.Enums;

namespace KL.Caller.Leads.Models.Entities;

public class SipProvider
{
    public long Id { get; set; }
    public string ProviderName { get; set; } = null!;
    public string ProviderAddress { get; set; } = null!;
    public string ProviderUserName { get; set; } = null!;
    public SipProviderStatus Status { get; set; }
}
