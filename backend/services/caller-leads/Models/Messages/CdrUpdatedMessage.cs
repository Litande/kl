using KL.Caller.Leads.Enums;

namespace KL.Caller.Leads.Models.Messages;

public record CdrUpdatedMessage(
    string? SessionId,
    long ClientId,
    string? LeadCountry,
    long? UserId,
    DateTimeOffset OriginatedAt,
    DateTimeOffset? CallHangupAt,
    DateTimeOffset? LeadAnswerAt,
    DateTimeOffset? UserAnswerAt,
    LeadStatusTypes? LeadStatusAfter
);