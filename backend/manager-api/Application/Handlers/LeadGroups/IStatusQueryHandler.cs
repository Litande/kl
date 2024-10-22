using KL.Manager.API.Application.Models.Responses.LeadGroups;

namespace KL.Manager.API.Application.Handlers.LeadGroups;

public interface IStatusQueryHandler
{
    Task<IReadOnlyCollection<LeadGroup>> Handle(long clientId, CancellationToken ct = default);
}
