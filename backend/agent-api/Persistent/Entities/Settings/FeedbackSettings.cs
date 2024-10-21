using Plat4Me.DialAgentApi.Application.Enums;

namespace Plat4Me.DialAgentApi.Persistent.Entities.Settings;

public record FeedbackSettings(
    long PageTimeout,
    long RedialsLimit,
    LeadStatusTypes DefaultStatus
);
