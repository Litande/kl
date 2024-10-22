using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models.Requests.Leads;

public record LeadsFilterRequest(
    string? LeadId,
    string? Phone,
    string? FirstName,
    string? LastName,
    List<string>? Country,
    string? Email,
    string? WeightLessThan,
    string? WeightMoreThan,
    string? Brand,
    FilterComparison<long>? TotalCalls,
    List<LeadStatusTypes>? LeadStatus,
    List<long>? AssignedAgent,
    LeadSearchGroupParams[]? Groups);
