﻿using Redis.OM.Modeling;

namespace KL.Manager.API.Persistent.Entities.Cache;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "LeadTracking" })]
public class LeadTrackingCache
{
    public LeadTrackingCache(
        long leadId,
        long? score = null)
    {
        LeadId = leadId;
        Score = score;
    }

    [RedisIdField]
    [Indexed]
    public long LeadId { get; set; }
    public long? Score { get; set; }
}
