namespace Plat4Me.DialClientApi.Application.Models.Messages.RuleEngine;

public record LeadsStatisticUpdateMessage(
    long ClientId,
    string Initiator);
