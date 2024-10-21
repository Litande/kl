using Plat4Me.DialRuleEngine.Application.Enums;

namespace Plat4Me.DialRuleEngine.Application.Models.Messages;

public record LeadFeedbackProcessedMessage(
    long ClientId,
    long LeadId,
    LeadStatusTypes Status,
    long UserId,
    string Comment
);