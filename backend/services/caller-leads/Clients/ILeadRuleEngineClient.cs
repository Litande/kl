using KL.Caller.Leads.Models.Requests;
using KL.Caller.Leads.Models.Responses;

namespace KL.Caller.Leads.Clients;

public interface ILeadRuleEngineClient
{
    Task<GetNextLeadResponse?> GetNextLead(long clientId, GetNextLeadRequest request, CancellationToken ct = default);
}
