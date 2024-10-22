namespace KL.Engine.Rule.Models;

public record QueuesUpdatedMessage(
    long ClientId,
    IReadOnlyCollection<long>? QueueIds = null)
{
    public string Initiator => nameof(KL.Engine.Rule);
}
