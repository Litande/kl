﻿namespace Plat4Me.DialLeadProvider.Persistent.Entities;

public class Client
{
    public long Id { get; set; }
    public virtual ICollection<DataSource> DataSources { get; set; } = null!;
}