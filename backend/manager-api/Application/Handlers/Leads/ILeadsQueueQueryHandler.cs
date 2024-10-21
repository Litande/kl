using Plat4Me.DialClientApi.Application.Models.Requests;
using Plat4Me.DialClientApi.Application.Models.Responses;
using Plat4Me.DialClientApi.Application.Models.Responses.Leads;

namespace Plat4Me.DialClientApi.Application.Handlers.Leads;

public interface ILeadsQueueQueryHandler
{
    Task<IReadOnlyCollection<LeadQueueItem>> Handle(long clientId, CancellationToken ct = default);
    Task<PaginatedResponse<LeadItem>?> Handle(long clientId, long queueId, PaginationRequest pagination, CancellationToken ct = default);
}
