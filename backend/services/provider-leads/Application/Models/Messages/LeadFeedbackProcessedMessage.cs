using Plat4Me.DialLeadProvider.Application.Enums;

namespace Plat4Me.DialLeadProvider.Application.Models.Messages;

public record LeadFeedbackProcessedMessage(
    long ClientId,
    long LeadId,
    LeadStatusTypes Status,
    long UserId,
    string Comment
);