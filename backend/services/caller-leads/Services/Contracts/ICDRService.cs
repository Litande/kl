using KL.Caller.Leads.Enums;
using KL.Caller.Leads.Models;
using KL.Caller.Leads.Models.Entities;
using KL.Caller.Leads.Models.Messages;

namespace KL.Caller.Leads.Services.Contracts;

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
