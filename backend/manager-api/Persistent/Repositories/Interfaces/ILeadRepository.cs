using System.Text.Json;
using KL.Manager.API.Application.Enums;
using KL.Manager.API.Application.Models.Requests;
using KL.Manager.API.Application.Models.Requests.Leads;
using KL.Manager.API.Application.Models.Responses;
using KL.Manager.API.Application.Models.Responses.Leads;
using KL.Manager.API.Persistent.Entities;
using KL.Manager.API.Persistent.Entities.Projections;

namespace KL.Manager.API.Persistent.Repositories.Interfaces;

public interface ILeadRepository
{
    Task<Lead?> GetById(long clientId, long leadId, CancellationToken ct = default);
    Task<Lead?> GetByIdAsNoTracking(long clientId, long leadId, CancellationToken ct = default);
    Task<LeadShortInfo?> UpdateLeadAssignment(long clientId, long userId, long leadId, long? assignedUserId, CancellationToken ct = default);
    Task<LeadShortInfo?> UpdateLeadStatus(long clientId, long userId, long leadId, LeadStatusTypes status, CancellationToken ct = default);
    Task<PaginatedResponse<Dictionary<string, JsonElement?>>> SearchLeads(long clientId, PaginationRequest pagination, LeadsFilterRequest? filter = null, CancellationToken ct = default);
    Task<IDictionary<long, LeadInfoProjection>> GetLeadInfoByIds(long clientId, IEnumerable<long> leadIds, CancellationToken ct = default);
    Task<LeadInfoProjection?> GetLeadInfoById(long clientId, long leadId, CancellationToken ct = default);
}
