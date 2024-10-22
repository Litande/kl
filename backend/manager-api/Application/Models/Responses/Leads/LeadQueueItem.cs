using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models.Responses.Leads;

public record LeadQueueItem(
    string GroupName,
    int LeadsCount,
    IReadOnlyCollection<LeadItem> Leads);

public record LeadItem(
    long LeadId,
    long LeadScore,
    LeadSystemStatusTypes? LeadSystemStatus,
    LeadStatusTypes LeadStatus);
