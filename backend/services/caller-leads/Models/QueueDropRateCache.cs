﻿using Redis.OM.Modeling;

namespace Plat4Me.DialLeadCaller.Application.Models;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "QueueDropRate" })]
public class QueueDropRateCache
{
    [Indexed]
    public long ClientId { get; set; }

    [RedisIdField] [Indexed]
    public long QueueId { get; set; }

    public double DropRate { get; set; }
}
