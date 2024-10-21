namespace Plat4Me.DialClientApi.Application.Handlers;

public interface ICallFinishByManagerHandler
{
    Task Handle(string sessionId, CancellationToken ct = default);
}