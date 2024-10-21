using Plat4Me.DialClientApi.Application.Models.Responses.AgentTrackings;
using Plat4Me.DialClientApi.Persistent.Entities;
using Plat4Me.DialClientApi.Persistent.Entities.Cache;
using Plat4Me.DialClientApi.Persistent.Entities.Projections;
using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Extensions;

public static class AgentTrackingExtensions
{
    public static AgentTrackingResponse Map(
        AgentInfoProjection agent,
        AgentStateCache? agentCache,
        CallInfoCache? callInfoCache,
        LeadInfoProjection? leadInfo,
        LeadQueue? leadQueue)
    {
        var result = new AgentTrackingResponse
        {
            Id = agent.AgentId,
            Name = agent.FullName(),
            Groups = agent.LeadQueues,
            TeamIds = agent.TeamIds,
            Tags = agent.Tags
        };

        if (agentCache is null)
            return result;

        result.State = agentCache.AgentStatus == AgentInternalStatusTypes.Offline ? AgentStatusTypes.Offline : agentCache.AgentDisplayStatus;

        if (callInfoCache is null)
            return result;

        result.Weight = callInfoCache.LeadScore;
        result.PhoneNumber = callInfoCache.LeadPhone;
        result.ManagerRtcUrl = callInfoCache.ManagerRtcUrl;
        result.CallOriginatedAt = callInfoCache.CallOriginatedAt.FromUnixTimeSeconds();
        result.LeadAnsweredAt = callInfoCache.LeadAnsweredAt.FromUnixTimeSeconds();
        result.AgentAnsweredAt = callInfoCache.AgentAnsweredAt.FromUnixTimeSeconds();
        result.CallFinishedAt = callInfoCache.CallFinishedAt.FromUnixTimeSeconds();
        result.CallType = callInfoCache.CallType;

        if (!callInfoCache.LeadId.HasValue || leadInfo is null)
            return result;

        result.LeadId = leadInfo.LeadId;
        result.Source = leadInfo.BrandName;
        result.LeadStatus = leadInfo.LeadStatus.ToDescription();
        result.LeadName = leadInfo.FullName();
        result.AffiliateId = leadInfo.AffiliateId;
        result.RegistrationTime = leadInfo.RegistrationTime;
        result.Country = leadInfo.CountryCode;

        if (leadQueue is null)
            return result;

        result.LeadGroup = leadQueue.Name;
        return result;
    }

    public static AgentTrackingResponse Map(
       AgentInfoProjection agent,
       IDictionary<long, AgentStateCache> agentCaches,
       IDictionary<string, CallInfoCache> callInfoCaches,
       IDictionary<long, LeadInfoProjection> leads,
       IDictionary<long, LeadQueue> leadQueues)
    {
        var result = new AgentTrackingResponse
        {
            Id = agent.AgentId,
            Name = agent.FullName(),
            Groups = agent.LeadQueues,
            TeamIds = agent.TeamIds,
            Tags = agent.Tags
        };

        if (!agentCaches.TryGetValue(agent.AgentId, out var agentCache))
            return result;

        result.State = agentCache.AgentStatus == AgentInternalStatusTypes.Offline ? AgentStatusTypes.Offline : agentCache.AgentDisplayStatus;

        if (agentCache.CallSession is null || !callInfoCaches.TryGetValue(agentCache.CallSession, out var callInfoCache))
            return result;

        result.Weight = callInfoCache.LeadScore;
        result.PhoneNumber = callInfoCache.LeadPhone;
        result.ManagerRtcUrl = callInfoCache.ManagerRtcUrl;
        result.CallOriginatedAt = callInfoCache.CallOriginatedAt.FromUnixTimeSeconds();
        result.LeadAnsweredAt = callInfoCache.LeadAnsweredAt.FromUnixTimeSeconds();
        result.AgentAnsweredAt = callInfoCache.AgentAnsweredAt.FromUnixTimeSeconds();
        result.CallFinishedAt = callInfoCache.CallFinishedAt.FromUnixTimeSeconds();
        result.CallType = callInfoCache.CallType;

        if (!callInfoCache.LeadId.HasValue || !leads.TryGetValue(callInfoCache.LeadId.Value, out var leadInfo))
            return result;

        result.LeadId = leadInfo.LeadId;
        result.Source = leadInfo.BrandName;
        result.LeadStatus = leadInfo.LeadStatus.ToDescription();
        result.LeadName = leadInfo.FullName();
        result.AffiliateId = leadInfo.AffiliateId;
        result.RegistrationTime = leadInfo.RegistrationTime;
        result.Country = leadInfo.CountryCode;

        if (!leadQueues.TryGetValue(leadInfo.LeadId, out var leadQueue))
            return result;

        result.LeadGroup = leadQueue.Name;
        return result;
    }


}
