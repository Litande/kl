using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;
using Plat4Me.DialLeadCaller.Application.App;
using Plat4Me.DialLeadCaller.Application.Enums;
using Plat4Me.DialLeadCaller.Application.Extensions;
using Plat4Me.DialLeadCaller.Application.Models;
using Plat4Me.DialLeadCaller.Application.Models.Entities;
using Plat4Me.DialLeadCaller.Application.Models.Messages;
using Plat4Me.DialLeadCaller.Application.Repositories;
using Plat4Me.DialLeadCaller.Application.Services.Contracts;

namespace Plat4Me.DialLeadCaller.Application.Services;

public class CDRService : ICDRService
{
    private readonly ICDRRepository _cdrRepository;
    private readonly INatsPublisher _natsPublisher;
    private readonly PubSubjects _pubSubjects;

    const string CDR_LOCK_PREFIX = "cdr_lock_";

    public string LockPrefix => CDR_LOCK_PREFIX;

    public CDRService(
        ICDRRepository cdrRepository,
        INatsPublisher natsPublisher,
        IOptions<PubSubjects> options)
    {
        _cdrRepository = cdrRepository;
        _natsPublisher = natsPublisher;
        _pubSubjects = options.Value;
    }

    public async Task<CallDetailRecord?> Update(CalleeAnsweredMessage message, CancellationToken ct = default)
    {
        var cdr = await _cdrRepository.Update(message, ct);
        await PublishCdrUpdated(cdr.MapToMessage());
        return cdr;
    }

    public async Task<CallDetailRecord?> Update(CallFinishedMessage message, CancellationToken ct = default)
    {
        var cdr = await _cdrRepository.Update(message, ct);
        await PublishCdrUpdated(cdr.MapToMessage());
        return cdr;
    }

    public async Task<CallDetailRecord?> Update(CallFinishedRecordsMessage message, CancellationToken ct = default)
    {
        var cdr = await _cdrRepository.Update(message, ct);
        await PublishCdrUpdated(cdr.MapToMessage());
        return cdr;
    }

    public async Task<CallDetailRecord?> Update(MixedRecordReadyMessage message, CancellationToken ct = default)
    {
        var cdr = await _cdrRepository.Update(message, ct);
        await PublishCdrUpdated(cdr.MapToMessage());
        return cdr;
    }

    public async Task<CallDetailRecord?> Update(DroppedAgentMessage message, CancellationToken ct = default)
    {
        var cdr = await _cdrRepository.Update(message, ct);
        await PublishCdrUpdated(cdr.MapToMessage());
        return cdr;
    }

    public async Task<CallDetailRecord> Add(CallDetailRecord record, CancellationToken ct = default)
    {
        var cdr = await _cdrRepository.Add(record, ct);
        await PublishCdrCreated(cdr.MapToMessage());
        return cdr;
    }

    public async Task<CallDetailRecord> Add(
        CallToRequest request,
        string leadPhone,
        string sessionId,
        CancellationToken ct = default)
    {
        var cdr = await _cdrRepository.Add(request, leadPhone, sessionId, ct);
        await PublishCdrCreated(cdr.MapToMessage());
        return cdr;
    }

    public async Task<CallDetailRecord?> Update(
        string sessionId,
        LeadStatusTypes leadStatus,
        CancellationToken ct = default)
    {
        var cdr = await _cdrRepository.Update(sessionId, leadStatus, ct);
        await PublishCdrUpdated(cdr.MapToMessage());
        return cdr;
    }

    public async Task Update(CallDetailRecord cdr, long userId, CancellationToken ct = default)
    {
        await _cdrRepository.Update(cdr, userId, ct);
        await PublishCdrUpdated(cdr.MapToMessage());
    }

    public async Task<CallDetailRecord?> GetBySessionId(
        string sessionId, CancellationToken ct = default)
    {
        var cdr = await _cdrRepository.GetBySessionId(sessionId, ct);
        return cdr;
    }

    private async Task PublishCdrUpdated(CdrUpdatedMessage? message)
    {
        if (message is null) return;
        await _natsPublisher.PublishAsync(_pubSubjects.CdrUpdated, message);
    }

    private async Task PublishCdrCreated(CdrUpdatedMessage message)
    {
        await _natsPublisher.PublishAsync(_pubSubjects.CdrInserted, message);
    }
}
