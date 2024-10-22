using KL.Manager.API.Application.Models.Requests;
using KL.Manager.API.Application.Models.Responses;
using KL.Manager.API.Application.Models.Responses.Leads;

namespace KL.Manager.API.Application.Handlers.Leads;

public interface ILeadsQueueQueryHandler
{
    Task<IReadOnlyCollection<LeadQueueItem>> Handle(long clientId, CancellationToken ct = default);
    Task<PaginatedResponse<LeadItem>?> Handle(long clientId, long queueId, PaginationRequest pagination, CancellationToken ct = default);
}
