using Plat4Me.DialClientApi.Application.Models.Responses.Leads;
using Plat4Me.DialClientApi.Persistent.Entities;
using Plat4Me.DialClientApi.Persistent.Entities.Cache;
using Plat4Me.DialClientApi.Persistent.Entities.Projections;

namespace Plat4Me.DialClientApi.Application.Extensions;

public static class LeadExtensions
{
    public static LeadShortInfo ToLeadShortInfoResponse(this Lead lead)
    {
        return new LeadShortInfo(
            lead.Id,
            lead.FullName(),
            lead.Status,
            null,
            null,
            null,
            null,
            lead.AssignedAgentId?.ToString(),
            lead.RegistrationTime,
            lead.Phone,
            lead.CountryCode
        );
    }

    public static LeadShortInfo ToLeadShortInfoResponse(this Lead lead, CallInfoCache? cachedInfo,
        LeadQueue? leadQueue)
    {
        return new LeadShortInfo(
            lead.Id,
            lead.FullName(),
            lead.Status,
            cachedInfo?.LeadAnsweredAt.FromUnixTimeSeconds(),
            cachedInfo?.AgentAnsweredAt.FromUnixTimeSeconds(),
            leadQueue?.Name,
            null,
            lead.AssignedAgentId?.ToString(),
            lead.RegistrationTime,
            lead.Phone,
            lead.CountryCode
        );
    }

    public static string FullName(this Lead lead)
        => $"{lead.FirstName} {lead.LastName}".Trim();

    public static string FullName(this LeadInfoProjection lead)
        => $"{lead.FirstName} {lead.LastName}".Trim();
}
