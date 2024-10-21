using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Models.Requests.Leads;

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
