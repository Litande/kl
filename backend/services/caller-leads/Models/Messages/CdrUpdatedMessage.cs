using Plat4Me.DialLeadCaller.Application.Enums;

namespace Plat4Me.DialLeadCaller.Application.Models.Messages;

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