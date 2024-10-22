using KL.Provider.Leads.Application.Enums;
using KL.Provider.Leads.Application.Models;

namespace KL.Provider.Leads.Persistent.Repositories.Interfaces;

public interface ILeadHistoryRepository
{
    Task AddLeadHistory(
        long leadId,
        IEnumerable<ValueChanges<object?>> changes,
        DateTimeOffset createdAt,
        long? createdBy = null,
        CancellationToken ct = default);

    Task AddLeadHistory<T>(
        long leadId,
        LeadHistoryActionType type,
        ValueChanges<T?> changes,
        DateTimeOffset createdAt,
        long? createdBy = null,
        CancellationToken ct = default);
}