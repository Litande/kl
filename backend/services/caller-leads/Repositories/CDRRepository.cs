using System.Text.Json;
using KL.Caller.Leads.Enums;
using KL.Caller.Leads.Extensions;
using KL.Caller.Leads.Models;
using KL.Caller.Leads.Models.Entities;
using KL.Caller.Leads.Models.Messages;

namespace KL.Caller.Leads.Repositories;

public class CDRRepository : ICDRRepository
{
    private readonly DialDbContext _context;

    private readonly IUserRepository _userRepository;

    public CDRRepository(
        DialDbContext context,
        IUserRepository userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }

    public async Task<CallDetailRecord?> GetById(
        long cdrId,
        CancellationToken ct = default)
    {
        return await _context.CallDetailRecords
           .Where(x => x.Id == cdrId)
           .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyCollection<CallDetailRecord>> GetForPeriod(
        long clientId,
        DateTimeOffset from,
        CancellationToken ct = default)
    {
        var entities = await _context.CallDetailRecords
            .Where(r => r.ClientId == clientId
                        && r.CallType == CallType.Predictive
                        && r.OriginatedAt > from
                        && r.CallHangupAt.HasValue)
            .ToArrayAsync(ct);
        return entities;
    }

    public async Task<CallDetailRecord?> GetBySessionId(
        string sessionId,
        CancellationToken ct = default)
    {
        var cdr = await _context.CallDetailRecords
            .Where(x => x.SessionId == sessionId)
            .FirstOrDefaultAsync(ct);

        return cdr;
    }

    public async Task<CallDetailRecord> Add(
        CallDetailRecord record,
        CancellationToken ct = default)
    {
        await _context.CallDetailRecords.AddAsync(record, ct);
        await _context.SaveChangesAsync(ct);
        return record;
    }

    public async Task<CallDetailRecord> Add(
       CallToRequest request,
       string leadPhone,
       string sessionId,
       CancellationToken ct = default)
    {
        var lead = request.LeadId is null
            ? null
            : await _context.Leads
                .Include(x => x.DataSource)
                .Where(x => x.ClientId == request.ClintId
                            && x.Id == request.LeadId)
                .FirstOrDefaultAsync(ct);

        var leadQueue = request.LeadQueueId is null
            ? null
            : await _context.LeadQueues
                .Where(x => x.ClientId == request.ClintId
                            && x.Id == request.LeadQueueId)
                .FirstOrDefaultAsync(ct);

        var user = await _userRepository.Get(request.ClintId, request.AgentId, ct);
        var metadata = new CallDetailRecordMetadata
        {
            Users = new List<CallDetailRecordUser>
            {
                new(request.AgentId, user.FullName())
            }
        };

        var cdr = new CallDetailRecord
        {
            ClientId = request.ClintId,
            LastUserId = request.AgentId,
            LastUserName = user.FullName(),
            LeadId = request.LeadId,
            LeadPhone = leadPhone,
            CallType = request.CallType,
            LeadQueueId = request.LeadQueueId,
            LeadQueueName = leadQueue?.Name,
            SessionId = sessionId,
            OriginatedAt = DateTimeOffset.UtcNow,
            MetaData = JsonSerializer.Serialize(metadata, JsonSettingsExtensions.Default),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            BridgeId = request.BridgeId
        };

        if (lead is not null)
        {
            cdr.Brand = lead.DataSource.Name;
            cdr.LeadName = lead.FullName();
            cdr.LeadCountry = lead.CountryCode;
            cdr.LeadStatusBefore = lead.Status;
        }

        await Add(cdr, ct);
        return cdr;
    }

    public async Task<CallDetailRecord?> Update(
        CalleeAnsweredMessage message,
        CancellationToken ct = default)
    {
        var cdr = await GetBySessionId(message.SessionId, ct);
        if (cdr is null) return cdr;

        //cdr.ClientId = message.ClientId; // throw error if other client
        cdr.UserAnswerAt = message.AgentAnswerAt;
        cdr.LeadAnswerAt = message.LeadAnswerAt;
        cdr.OriginatedAt = message.CallOriginatedAt;
        cdr.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(ct);
        return cdr;
    }

    public async Task<CallDetailRecord?> Update(
        CallFinishedMessage message,
        CancellationToken ct = default)
    {
        var cdr = await GetBySessionId(message.SessionId, ct);
        if (cdr is null) return cdr;

        cdr.LeadAnswerAt = message.LeadAnswerAt;
        cdr.UserAnswerAt = message.AgentAnswerAt;
        cdr.OriginatedAt = message.CallOriginatedAt;
        cdr.CallHangupAt = message.CallHangupAt;
        cdr.CallHangupStatus = message.ReasonCode;
        cdr.SipProviderId = message.SipProviderId;
        cdr.SipErrorCode = message.SipErrorCode;
        cdr.CallDuration = CalculateCallDuration(message);
        cdr.UpdatedAt = DateTimeOffset.UtcNow;

        var metadata = cdr.MetaData is null
                ? new CallDetailRecordMetadata()
                : JsonSerializer.Deserialize<CallDetailRecordMetadata>(cdr.MetaData, JsonSettingsExtensions.Default);

        metadata!.ReasonDetails = message.ReasonDetails;
        metadata!.AgentComment = message.AgentComment;
        metadata!.ManagerComment = message.ManagerComment;
        if (message.AgentWasDropped && metadata!.DroppedInfo is not null)
        {

            metadata.DroppedInfo = metadata.DroppedInfo with { Comment = message.ManagerComment };
        }
        cdr.MetaData = JsonSerializer.Serialize(metadata, JsonSettingsExtensions.Default);
        await _context.SaveChangesAsync(ct);
        return cdr;
    }

    public long? CalculateCallDuration(CallFinishedMessage record)
    {
        if (record.LeadAnswerAt is null || record.AgentAnswerAt is null)
            return null;

        var maxDate = record.LeadAnswerAt > record.AgentAnswerAt
            ? record.LeadAnswerAt.Value : record.AgentAnswerAt.Value;

        var duration = (record.CallHangupAt - maxDate).TotalSeconds;

        return (long)duration;
    }

    public async Task<CallDetailRecord?> Update(
        CallFinishedRecordsMessage message,
        CancellationToken ct = default)
    {
        var cdr = await GetBySessionId(message.SessionId, ct);
        if (cdr is null) return cdr;

        cdr.RecordLeadFile = message.RecordLeadFile;
        cdr.RecordUserFiles = message.RecordUserFiles is null
            ? null
            : string.Join(';', message.RecordUserFiles);
        cdr.RecordManagerFiles = message.RecordManagerFiles is null
            ? null
            : string.Join(';', message.RecordManagerFiles);
        cdr.RecordMixedFile = message.RecordMixedFile;
        cdr.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(ct);
        return cdr;
    }

    public async Task<CallDetailRecord?> Update(
        string sessionId,
        LeadStatusTypes leadStatus,
        CancellationToken ct = default)
    {
        var cdr = await GetBySessionId(sessionId, ct);
        if (cdr is null) return cdr;

        cdr.LeadStatusAfter = leadStatus;
        cdr.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(ct);
        return cdr;
    }

    public async Task<IReadOnlyCollection<CallDetailRecord>> GetCallDetailsForCalculations(
        long clientId,
        DateTimeOffset startFrom,
        IEnumerable<LeadStatusTypes> statuses,
        CancellationToken ct = default)
    {
        var callDetailRecords = await _context.CallDetailRecords
            .Include(x => x.Lead)
            .Where(x => x.ClientId == clientId
                        && x.LeadId.HasValue
                        && !string.IsNullOrEmpty(x.LeadCountry)
                        && x.OriginatedAt >= startFrom
                        && statuses.Contains(x.Lead!.Status)
            )
            .ToArrayAsync(ct);

        return callDetailRecords;
    }

    public async Task<CallDetailRecord?> Update(
        MixedRecordReadyMessage message,
        CancellationToken ct = default)
    {
        var cdr = await GetBySessionId(message.SessionId, ct);
        if (cdr is null) return cdr;

        cdr.RecordMixedFile = message.RecordMixedFile;
        cdr.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(ct);
        return cdr;
    }

    public async Task<CallDetailRecord?> Update(DroppedAgentMessage message, CancellationToken ct = default)
    {
        var cdr = await GetBySessionId(message.SessionId, ct);
        if (cdr is null) return cdr;

        var metadata = cdr.MetaData is null
            ? new CallDetailRecordMetadata()
            : JsonSerializer.Deserialize<CallDetailRecordMetadata>(cdr.MetaData, JsonSettingsExtensions.Default);

        metadata!.DroppedInfo = new CallDetailRecordDroppedInfo(
                message.AgentId,
                message.DroppedBy,
                message.DroppedAt,
                message.Comment);


        cdr.MetaData = JsonSerializer.Serialize(metadata, JsonSettingsExtensions.Default);
        cdr.DroppedByManagerId = message.DroppedBy;
        cdr.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(ct);
        return cdr;
    }

    public async Task Update(
        CallDetailRecord cdr,
        long userId,
        CancellationToken ct = default)
    {
        var metadata = cdr.MetaData is null
            ? new CallDetailRecordMetadata()
            : JsonSerializer.Deserialize<CallDetailRecordMetadata>(cdr.MetaData, JsonSettingsExtensions.Default);

        var user = await _userRepository.Get(cdr.ClientId, userId, ct);

        metadata!.Users.Add(new CallDetailRecordUser(userId, user.FullName()));
        cdr.LastUserId = userId;
        cdr.IsReplacedUser = true;
        cdr.MetaData = JsonSerializer.Serialize(metadata, JsonSettingsExtensions.Default);
        cdr.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyCollection<CallDetailRecord>> GetIncompleteByBridgeIds(
        IReadOnlyCollection<string> bridgeIds,
        CancellationToken ct = default)
    {
        var callDetailRecords = await _context.CallDetailRecords.
            Where(x =>
                    x.CallHangupStatus == null
                    && bridgeIds.Contains(x.BridgeId))
            .ToArrayAsync(ct);
        return callDetailRecords;
    }
}
