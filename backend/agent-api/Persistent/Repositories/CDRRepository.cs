using KL.Agent.API.Application.Enums;
using KL.Agent.API.Application.Models.Requests;
using KL.Agent.API.Application.Models.Responses;
using KL.Agent.API.Persistent.Entities;
using KL.Agent.API.Persistent.Entities.Projections;
using KL.Agent.API.Persistent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KL.Agent.API.Persistent.Repositories;

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
            .AsNoTracking()
            .Where(x => x.ClientId == currentClientId
                        && x.Id == cdrId)
            .FirstOrDefaultAsync(ct);
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

    public async Task<PaginatedResponse<CDRHistoryProjection>> GetAllByUserId(
        long currentClientId,
        long currentAgentId,
        PaginationRequest pagination,
        CancellationToken ct = default)
    {
        var q = _context.CallDetailRecords
            .Include(x => x.Lead)
            .Where(x => x.ClientId == currentClientId
                        && x.UserId == currentAgentId
                        && x.CallHangupAt.HasValue);

        var rq = q.Select(r => new CDRHistoryProjection
        {
            Id = r.Id,
            FirstName = r.Lead!.FirstName,
            LastName = r.Lead.LastName,
            LeadPhone = r.LeadPhone,
            Country = r.LeadCountry,
            LastActivity = r.CallHangupAt
        });

        return await CreatePaginatedResponse(rq, pagination, ct);
    }

    public async Task<PaginatedResponse<CDRAgentHistoryProjection>> GetAllByLeadPhone(
        long currentClientId,
        string phoneNumber,
        PaginationRequest pagination,
        CancellationToken ct = default)
    {
        var q = _context.CallDetailRecords
            .Where(x => x.ClientId == currentClientId
                        && x.LeadPhone == phoneNumber);

        var rq = q.Select(r => new CDRAgentHistoryProjection
        {
            Id = r.Id,
            FirstName = r.User!.FirstName,
            LastName = r.User.LastName,
            Phone = r.LeadPhone,
            CallDuration = r.CallDuration,
            Date = r.CallHangupAt ?? r.LeadAnswerAt ?? r.UserAnswerAt,
            LeadStatusAfter = r.LeadStatusAfter,
        });

        return await CreatePaginatedResponse(rq, pagination, ct);
    }

    public async Task<IReadOnlyCollection<CDRFeedbackTimeoutProjection>> GetRecordsForFeedBack(
        CancellationToken ct = default)
    {
        var query = _context.CallDetailRecords.Where(x =>
            x.Lead != null
            && x.User != null
            && x.CallType == CallType.Predictive
            && x.CallHangupAt != null
            && x.UserAnswerAt != null
            && x.Lead.SystemStatus == LeadSystemStatusTypes.WaitingFeedback);

        var result = await query.Select(r => new CDRFeedbackTimeoutProjection()
        {
            AgentId = r.UserId!.Value,
            LeadId = r.LeadId!.Value,
            ClientId = r.ClientId!.Value,
            SessionId = r.SessionId!
        }).ToArrayAsync(ct);

        return result;
    }
}
