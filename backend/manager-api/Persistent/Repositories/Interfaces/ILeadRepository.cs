using System.Text.Json;
using System.Text.Json.Nodes;
using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Application.Models.Requests;
using Plat4Me.DialClientApi.Application.Models.Requests.Leads;
using Plat4Me.DialClientApi.Application.Models.Responses;
using Plat4Me.DialClientApi.Application.Models.Responses.Leads;
using Plat4Me.DialClientApi.Persistent.Entities;
using Plat4Me.DialClientApi.Persistent.Entities.Projections;

namespace Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

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
