namespace Plat4Me.DialClientApi.Application.Models.Messages.LeadGroups;

public record QueuesUpdatedMessage(
    long ClientId,
    string Initiator,
    IReadOnlyCollection<long>? QueueIds = null);
