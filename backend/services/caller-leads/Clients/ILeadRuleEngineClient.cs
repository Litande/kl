using Plat4Me.DialLeadCaller.Application.Models.Requests;
using Plat4Me.DialLeadCaller.Application.Models.Responses;

namespace Plat4Me.DialLeadCaller.Application.Clients;

public interface ILeadRuleEngineClient
{
    Task<GetNextLeadResponse?> GetNextLead(long clientId, GetNextLeadRequest request, CancellationToken ct = default);
}
