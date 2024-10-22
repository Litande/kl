using KL.Statistics.Application.Common.Enums;

namespace KL.Statistics.Application.Models.Messages;

public record CdrUpdatedMessage(
    string SessionId,
    long ClientId,
    string? LeadCountry,
    long? UserId,
    DateTimeOffset OriginatedAt,
    DateTimeOffset? CallHangupAt,
    DateTimeOffset? LeadAnswerAt,
    DateTimeOffset? UserAnswerAt,
    LeadStatusTypes? LeadStatusAfter);