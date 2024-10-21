namespace Plat4Me.DialLeadCaller.Application.Models.Messages;

public record QueuesUpdatedMessage(
    long ClientId,
    IReadOnlyCollection<long>? QueueIds = null)
{
    public string Initiator => nameof(DialLeadCaller);
}
