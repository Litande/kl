using Plat4Me.DialLeadCaller.Application.Enums;

namespace Plat4Me.DialLeadCaller.Application.Models.LeadStatisticCache;

public record LeadStatisticSettings(List<LeadStatusTypes> LeadStatuses);