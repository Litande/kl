using Plat4Me.Dial.Statistic.Api.Application.Common.Enums;

namespace Plat4Me.Dial.Statistic.Api.Application.Models.Messages;

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