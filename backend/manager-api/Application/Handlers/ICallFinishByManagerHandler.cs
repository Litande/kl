namespace KL.Manager.API.Application.Handlers;

public interface ICallFinishByManagerHandler
{
    Task Handle(string sessionId, CancellationToken ct = default);
}