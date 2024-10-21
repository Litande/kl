using Plat4Me.DialLeadProvider.Application.Enums;
using Plat4Me.DialLeadProvider.Application.Models;

namespace Plat4Me.DialLeadProvider.Persistent.Repositories.Interfaces;

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