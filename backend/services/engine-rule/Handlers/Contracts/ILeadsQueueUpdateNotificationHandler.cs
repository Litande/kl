namespace Plat4Me.DialRuleEngine.Application.Handlers.Contracts;

public interface ILeadsQueueUpdateNotificationHandler
{
    Task Process(long clientId);
    Task Process(long clientId, long queueId);
    Task Process(long clientId, long? queueId);
}
