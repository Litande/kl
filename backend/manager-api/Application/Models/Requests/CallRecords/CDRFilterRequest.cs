using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Models.Requests.CallRecords;

public record CDRFilterRequest(
    string? GroupName,
    string? LeadId,
    string? LeadPhone,
    string? UserId,
    string? UserName,
    List<string>? Country,
    CallType? CallType,
    List<LeadStatusTypes>? LeadStatusAfter,
    DateTime? FromDate,
    DateTime? TillDate,
    FilterComparison<int>? Duration
);
