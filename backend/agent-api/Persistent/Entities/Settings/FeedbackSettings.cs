using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Persistent.Entities.Settings;

public record FeedbackSettings(
    long PageTimeout,
    long RedialsLimit,
    LeadStatusTypes DefaultStatus
);
