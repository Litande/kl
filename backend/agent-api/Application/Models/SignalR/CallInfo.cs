using Plat4Me.DialAgentApi.Application.Enums;

namespace Plat4Me.DialAgentApi.Application.Models.SignalR;

public record CallInfo(
    long? Id,
    long? ClientId,
    long? DataSourceId,
    long? DuplicateOfId,
    string Phone,
    CallType CallType,
    string SessionId,
    DateTimeOffset? LastUpdateTime,
    string? ExternalId,
    string? FirstName,
    string? LastName,
    string? LanguageCode,
    string? CountryCode,
    LeadStatusTypes? Status,
    DateTimeOffset? LastTimeOnline,
    DateTimeOffset? RegistrationTime,
    long? AssignedUserId,
    bool? IsFixedAssigned,
    DateTimeOffset? FirstDepositTime,
    DateTimeOffset? RemindOn,
    DateTimeOffset CallOriginatedAt,
    DateTimeOffset? LeadAnsweredAt,
    DateTimeOffset? AgentAnsweredAt,
    DateTimeOffset? CallFinishAt,
    string? AgentRtcUrl,
    string? IframeUrl,
    IEnumerable<LeadStatusTypes>? AvailableStatuses,
    long CallAgainCount
    );
