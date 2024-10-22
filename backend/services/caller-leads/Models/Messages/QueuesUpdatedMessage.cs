namespace KL.Caller.Leads.Models.Messages;

public record QueuesUpdatedMessage(
    long ClientId,
    IReadOnlyCollection<long>? QueueIds = null)
{
    public string Initiator => nameof(KL.Caller);
}
