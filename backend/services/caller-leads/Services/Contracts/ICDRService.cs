using Plat4Me.DialLeadCaller.Application.Enums;
using Plat4Me.DialLeadCaller.Application.Models;
using Plat4Me.DialLeadCaller.Application.Models.Entities;
using Plat4Me.DialLeadCaller.Application.Models.Messages;

namespace Plat4Me.DialLeadCaller.Application.Services.Contracts;

public interface ICDRService
{
    Task<CallDetailRecord?> Update(CalleeAnsweredMessage message, CancellationToken ct = default);
    Task<CallDetailRecord?> Update(CallFinishedMessage message, CancellationToken ct = default);
    Task<CallDetailRecord?> Update(CallFinishedRecordsMessage message, CancellationToken ct = default);
    Task<CallDetailRecord?> Update(MixedRecordReadyMessage message, CancellationToken ct = default);
    Task<CallDetailRecord?> Update(DroppedAgentMessage message, CancellationToken ct = default);
    Task<CallDetailRecord> Add(CallDetailRecord record, CancellationToken ct = default);
    Task<CallDetailRecord> Add(CallToRequest request, string leadPhone, string sessionId, CancellationToken ct = default);
    Task<CallDetailRecord?> Update(string sessionId, LeadStatusTypes leadStatus, CancellationToken ct = default);
    Task Update(CallDetailRecord cdr, long userId, CancellationToken ct = default);
    Task<CallDetailRecord?> GetBySessionId(string sessionId, CancellationToken ct = default);
    string LockPrefix { get; }
}
