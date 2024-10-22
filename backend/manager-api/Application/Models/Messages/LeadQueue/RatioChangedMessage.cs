namespace KL.Manager.API.Application.Models.Messages.LeadQueue;

public record RatioChangedMessage(
    long ClientId,
    long QueueId,
    double Ratio)
{
    public string Initiator => nameof(KL.Manager.API);
}
