using KL.Engine.Rule.Enums;

namespace KL.Engine.Rule.Models.Messages;

public record LeadFeedbackProcessedMessage(
    long ClientId,
    long LeadId,
    LeadStatusTypes Status,
    long UserId,
    string Comment
);