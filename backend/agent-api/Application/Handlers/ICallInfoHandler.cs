using Plat4Me.DialAgentApi.Application.Models.Messages;
using Plat4Me.DialAgentApi.Application.Models;

namespace Plat4Me.DialAgentApi.Application.Handlers;

public interface ICallInfoHandler
{
    Task Handle(long agentId, CallInfo callInfo, CancellationToken ct = default);
}
