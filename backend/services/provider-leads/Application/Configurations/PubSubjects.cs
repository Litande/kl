﻿namespace Plat4Me.DialLeadProvider.Application.Configurations;

public class PubSubjects
{
    /// <summary>
    /// Use for notify other services that leads imported
    /// </summary>
    public string LeadsImported { get; set; } = null!;
}