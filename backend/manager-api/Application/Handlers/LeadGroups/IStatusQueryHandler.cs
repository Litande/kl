using Plat4Me.DialClientApi.Application.Models.Responses.LeadGroups;

namespace Plat4Me.DialClientApi.Application.Handlers.LeadGroups;

public interface IStatusQueryHandler
{
    Task<IReadOnlyCollection<LeadGroup>> Handle(long clientId, CancellationToken ct = default);
}
