using KL.Manager.API.Application.Models.Responses.Leads;

namespace KL.Manager.API.Application.Handlers.Leads;

public interface ILeadInfoQueryHandler
{
    Task<LeadShortInfo> Handle(long clientId, long leadId, CancellationToken ct = default);
}
