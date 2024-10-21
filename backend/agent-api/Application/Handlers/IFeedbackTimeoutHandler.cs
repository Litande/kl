namespace Plat4Me.DialAgentApi.Application.Handlers;

public interface IFeedbackTimeoutHandler
{
    Task Handle(long clientId, long agentId, string sessionId, DateTimeOffset callFinishedAt, CancellationToken ct = default);
    Task Handle();
}
