using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Models.Responses.Leads;
public record LeadShortInfo(
    long LeadId,
    string Name,
    LeadStatusTypes LeadStatus,
    DateTimeOffset? LeadAnsweredAt,
    DateTimeOffset? AgentAnsweredAt,
    string? LeadGroup,
    string? BrandName,
    string? AffiliateId,
    DateTimeOffset RegistrationTime,
    string PhoneNumber,
    string? Country);