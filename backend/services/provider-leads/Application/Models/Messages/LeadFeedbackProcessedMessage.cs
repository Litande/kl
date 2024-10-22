using KL.Provider.Leads.Application.Enums;

namespace KL.Provider.Leads.Application.Models.Messages;

public record LeadFeedbackProcessedMessage(
    long ClientId,
    long LeadId,
    LeadStatusTypes Status,
    long UserId,
    string Comment
);