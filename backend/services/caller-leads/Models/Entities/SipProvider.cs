﻿using Plat4Me.DialLeadCaller.Application.Enums;

namespace Plat4Me.DialLeadCaller.Application.Models.Entities;

public class SipProvider
{
    public long Id { get; set; }
    public string ProviderName { get; set; } = null!;
    public string ProviderAddress { get; set; } = null!;
    public string ProviderUserName { get; set; } = null!;
    public SipProviderStatus Status { get; set; }
}
