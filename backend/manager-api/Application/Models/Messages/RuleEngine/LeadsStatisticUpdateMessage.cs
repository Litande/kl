namespace KL.Manager.API.Application.Models.Messages.RuleEngine;

public record LeadsStatisticUpdateMessage(
    long ClientId,
    string Initiator);
