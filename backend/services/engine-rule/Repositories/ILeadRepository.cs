﻿using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.Models.Entities;

namespace Plat4Me.DialRuleEngine.Application.Repositories;

public interface ILeadRepository
{
    Task<IReadOnlyCollection<Client>> GetAllClients(CancellationToken ct = default);
    Task<IReadOnlyCollection<LeadStatusDto>> GetLeadsWithSystemStatus(CancellationToken ct = default);
    Task<IReadOnlyCollection<TrackedLead>> GetForPreProcessing(long clientId, CancellationToken ct = default);
    Task<IReadOnlyCollection<TrackedLead>> GetForPostProcessing(long clientId, IEnumerable<long>? leadIds = null, CancellationToken ct = default);
    Task<IReadOnlyCollection<TrackedLead>> GetForImportedProcessing(long clientId, IEnumerable<long>? leadIds = null, CancellationToken ct = default);
    Task<Lead?> UpdateSystemStatus(long leadId, LeadSystemStatusTypes? systemStatus, CancellationToken ct = default);
    Task ClearSystemStatuses(IEnumerable<long> leadIds, CancellationToken ct = default);
    Task UpdateLeads(IEnumerable<TrackedLead> leads, CancellationToken ct = default);
    Task UpdateFirstTimeQueued(IEnumerable<long> leadIds, CancellationToken ct = default);
    Task ResetAssignation(IEnumerable<long> agentIds, CancellationToken ct = default);
    Task AddLeadHistories(long leadId, LeadHistoryActionType actionType, long? createdBy = null, CancellationToken ct = default, params ValueChanges<object?>[] changes);
}
