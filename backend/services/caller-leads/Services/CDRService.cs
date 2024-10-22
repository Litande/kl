using KL.Caller.Leads.App;
using KL.Caller.Leads.Enums;
using KL.Caller.Leads.Extensions;
using KL.Caller.Leads.Models;
using KL.Caller.Leads.Models.Entities;
using KL.Caller.Leads.Models.Messages;
using KL.Caller.Leads.Repositories;
using KL.Caller.Leads.Services.Contracts;
using KL.Nats;
using Microsoft.Extensions.Options;

namespace KL.Caller.Leads.Services;

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
