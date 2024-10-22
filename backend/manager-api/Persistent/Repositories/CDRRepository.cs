using KL.Manager.API.Application.Enums;
using KL.Manager.API.Application.Models.Requests;
using KL.Manager.API.Application.Models.Requests.CallRecords;
using KL.Manager.API.Application.Models.Responses;
using KL.Manager.API.Persistent.Entities;
using KL.Manager.API.Persistent.Entities.Projections;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KL.Manager.API.Persistent.Repositories;

public class CDRRepository : RepositoryBase, ICDRRepository
{
    private readonly DialDbContext _context;

    public CDRRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<CallDetailRecord?> GetById(
        long currentClientId,
        long cdrId,
        CancellationToken ct = default)
    {
        return await _context.CallDetailRecords
            .Where(r => r.ClientId == currentClientId
                        && r.Id == cdrId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<PaginatedResponse<CDRProjection>> ListCalls(
        long currentClientId,
        PaginationRequest pagination,
        CDRFilterRequest? filter = null,
        CancellationToken ct = default)
    {
        var q = _context.CallDetailRecords
            .Where(x => x.ClientId == currentClientId
                        && x.CallHangupStatus != null
                        && x.CallHangupAt != null); //filter out active calls or bugged

        if (filter is not null)
        {
            if (!string.IsNullOrEmpty(filter.LeadId))
            {
                var pattern = FullSearchPattern(filter.LeadId);
                q = q.Where(r => EF.Functions.Like(r.LeadId, pattern));
            }
            if (!string.IsNullOrEmpty(filter.LeadPhone))
            {
                var pattern = FullSearchPattern(filter.LeadPhone);
                q = q.Where(r => EF.Functions.Like(r.LeadPhone, pattern));
            }
            if (!string.IsNullOrEmpty(filter.UserId))
            {
                var pattern = FullSearchPattern(filter.UserId);
                q = q.Where(r => EF.Functions.Like(r.UserId.ToString()!, pattern));
            }
            if (!string.IsNullOrEmpty(filter.UserName))
            {
                var pattern = FullSearchPattern(filter.UserName);
                q = q.Where(r => EF.Functions.Like(r.UserName!, pattern));
            }
            if (!string.IsNullOrEmpty(filter.GroupName))
            {
                var pattern = FullSearchPattern(filter.GroupName);
                q = q.Where(r => EF.Functions.Like(r.LeadQueueName!, pattern));
            }
            if (filter.Country is not null && filter.Country.Any())
            {
                q = q.Where(r => filter.Country.Contains(r.LeadCountry!));
            }
            if (filter.LeadStatusAfter is not null && filter.LeadStatusAfter.Any())
            {
                q = q.Where(r => filter.LeadStatusAfter.Contains(r.LeadStatusAfter!.Value));
            }
            if (filter.FromDate is not null)
            {
                q = q.Where(r => r.OriginatedAt >= filter.FromDate);
            }
            if (filter.TillDate is not null)
            {
                q = q.Where(r => r.OriginatedAt <= filter.TillDate);
            }
            if (filter.Duration is not null)
            {
                switch (filter.Duration.Operation)
                {
                    case FilterComparisonOperation.Equal:
                        q = q.Where(r => EF.Functions.DateDiffSecond(r.OriginatedAt, r.CallHangupAt) == filter.Duration.Value);
                        break;
                    case FilterComparisonOperation.MoreThan:
                        q = q.Where(r => EF.Functions.DateDiffSecond(r.OriginatedAt, r.CallHangupAt) > filter.Duration.Value);
                        break;
                    case FilterComparisonOperation.LessThan:
                        q = q.Where(r => EF.Functions.DateDiffSecond(r.OriginatedAt, r.CallHangupAt) < filter.Duration.Value);
                        break;
                }
            }
            if (filter.CallType is not null)
            {
                if (!Enum.IsDefined(filter.CallType.Value))
                    throw new ArgumentException($"{filter.CallType} is wrong {nameof(CallType)} type");

                q = q.Where(x => x.CallType == filter.CallType);
            }
        }

        var rq = q.Select(r => new CDRProjection
        {
            CallId = r.Id,
            LeadId = r.LeadId,
            LeadName = r.LeadName,
            LeadPhone = r.LeadPhone,
            LeadStatusAfter = r.LeadStatusAfter,
            UserId = r.UserId,
            UserName = r.UserName,
            Brand = r.Brand,
            GroupName = r.LeadQueueName,
            CallType = r.CallType,
            StartedAt = r.OriginatedAt,
            Duration = EF.Functions.DateDiffSecond(r.OriginatedAt, r.CallHangupAt)!.Value,
            BillDuration = r.LeadAnswerAt == null
                ? 0
                : EF.Functions.DateDiffSecond(r.LeadAnswerAt, r.CallHangupAt)!.Value,
            HangupStatus = r.CallHangupStatus!.Value,
            HasMixedRecord = r.RecordMixedFile != null
        });

        return await CreatePaginatedResponse(rq, pagination, ct);
    }

    public async Task<IReadOnlyCollection<CallDetailRecord>> GetCallsByPeriod(
        long clientId,
        DateTimeOffset fromDateTime,
        DateTimeOffset toDateTime,
        CancellationToken ct = default)
    {
        var calls = await _context.CallDetailRecords.AsNoTracking()
            .Where(c => c.ClientId == clientId
                        && c.OriginatedAt > fromDateTime
                        && c.OriginatedAt <= toDateTime)
            .ToListAsync(ct);

        return calls;
    }
}
