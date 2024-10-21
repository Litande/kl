using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Models.Responses.Leads;

public record LeadQueueItem(
    string GroupName,
    int LeadsCount,
    IReadOnlyCollection<LeadItem> Leads);

public record LeadItem(
    long LeadId,
    long LeadScore,
    LeadSystemStatusTypes? LeadSystemStatus,
    LeadStatusTypes LeadStatus);
