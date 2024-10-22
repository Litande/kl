using KL.Caller.Leads.Models;
using KL.Caller.Leads.Models.Messages;

namespace KL.Caller.Leads.Services.Contracts;

public interface ICallInfoService
{
    Task AddCallInfo(CallInitiatedMessage message, CancellationToken ct = default);
    Task UpdateCallInfo(CallFinishedMessage message, CancellationToken ct = default);
    Task UpdateCallInfo(CalleeAnsweredMessage message, CancellationToken ct = default);
    Task UpdateCallInfo(DroppedAgentMessage message, CancellationToken ct = default);
    Task ClearCallInfo(string sessionId, CancellationToken ct = default);
}
