namespace KL.Engine.Rule.Handlers.Contracts;

public interface ILeadsQueueUpdateNotificationHandler
{
    Task Process(long clientId);
    Task Process(long clientId, long queueId);
    Task Process(long clientId, long? queueId);
}
