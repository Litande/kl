namespace KL.Statistics.Application.Models.Messages;

public record LeadsStatisticUpdateMessage(
    long ClientId,
    string Initiator);
