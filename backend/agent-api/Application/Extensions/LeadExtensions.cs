using KL.Agent.API.Application.Enums;
using KL.Agent.API.Application.Models.SignalR;
using KL.Agent.API.Persistent.Entities;

namespace KL.Agent.API.Application.Extensions;

public static class LeadExtensions
{
    public static CallInfo ToCallInfo(this Lead? lead,
        string leadPhone,
        CallType callType,
        string sessionId,
        string? agentRtcUrl,
        DateTimeOffset callOriginatedAt,
        DateTimeOffset? leadAnsweredAt,
        DateTimeOffset? agentAnsweredAt,
        DateTimeOffset? callFinishAt,
        long callAgainCount,
        string? iframeUrl,
        IEnumerable<LeadStatusTypes>? availableStatuses
    )
        => new(
            Id: lead?.Id,
            ClientId: lead?.ClientId,
            DataSourceId: lead?.DataSourceId,
            DuplicateOfId: lead?.DuplicateOfId,
            Phone: leadPhone,
            CallType: callType,
            SessionId: sessionId,
            LastUpdateTime: lead?.LastUpdateTime,
            ExternalId: lead?.ExternalId,
            FirstName: lead?.FirstName,
            LastName: lead?.LastName,
            LanguageCode: lead?.LanguageCode,
            CountryCode: lead?.CountryCode,
            Status: lead?.Status,
            LastTimeOnline: lead?.LastTimeOnline,
            RegistrationTime: lead?.RegistrationTime,
            AssignedUserId: lead?.AssignedAgentId,
            IsFixedAssigned: lead?.AssignedAgentId.HasValue,
            FirstDepositTime: lead?.FirstDepositTime,
            RemindOn: lead?.RemindOn,
            CallOriginatedAt: callOriginatedAt,
            LeadAnsweredAt: leadAnsweredAt,
            AgentAnsweredAt: agentAnsweredAt,
            CallFinishAt: callFinishAt,
            AgentRtcUrl: agentRtcUrl,
            IframeUrl: iframeUrl,
            AvailableStatuses: availableStatuses,
            CallAgainCount: callAgainCount
        );
}
