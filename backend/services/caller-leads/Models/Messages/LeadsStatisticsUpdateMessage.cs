namespace Plat4Me.DialLeadCaller.Application.Models.Messages;

public record LeadsStatisticsUpdateMessage(long ClientId)
{
    public string Initiator => nameof(DialLeadCaller);
}
