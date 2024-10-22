using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models.Responses.Leads;
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