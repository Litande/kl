using KL.Agent.API.Application.Models;

namespace KL.Agent.API.Application.Handlers;

public interface ICallInfoHandler
{
    Task Handle(long agentId, CallInfo callInfo, CancellationToken ct = default);
}
