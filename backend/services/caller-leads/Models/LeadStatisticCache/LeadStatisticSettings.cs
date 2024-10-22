using KL.Caller.Leads.Enums;

namespace KL.Caller.Leads.Models.LeadStatisticCache;

public record LeadStatisticSettings(List<LeadStatusTypes> LeadStatuses);