namespace Plat4Me.Dial.Statistic.Api.Application.Models.Messages;

public record LeadsStatisticUpdateMessage(
    long ClientId,
    string Initiator);
