namespace KL.Caller.Leads.Models.Messages;

public record LeadsStatisticsUpdateMessage(long ClientId)
{
    public string Initiator => nameof(KL.Caller);
}
