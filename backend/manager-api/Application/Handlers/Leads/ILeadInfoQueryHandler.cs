using Plat4Me.DialClientApi.Application.Models.Responses.Leads;

namespace Plat4Me.DialClientApi.Application.Handlers.Leads;

public interface ILeadInfoQueryHandler
{
    Task<LeadShortInfo> Handle(long clientId, long leadId, CancellationToken ct = default);
}
