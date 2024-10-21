using Plat4Me.DialLeadCaller.Application.Models.Messages;
using Plat4Me.DialLeadCaller.Application.Models;

namespace Plat4Me.DialLeadCaller.Application.Services.Contracts;

public interface ICallInfoService
{
    Task AddCallInfo(CallInitiatedMessage message, CancellationToken ct = default);
    Task UpdateCallInfo(CallFinishedMessage message, CancellationToken ct = default);
    Task UpdateCallInfo(CalleeAnsweredMessage message, CancellationToken ct = default);
    Task UpdateCallInfo(DroppedAgentMessage message, CancellationToken ct = default);
    Task ClearCallInfo(string sessionId, CancellationToken ct = default);
}
