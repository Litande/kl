using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Application.Models;

namespace Plat4Me.DialClientApi.Persistent.Clients;

public interface IRuleEngineClient
{
    Task<OperationResponse?> ValidateRules(long clientId, RuleGroupTypes ruleType, string rules, CancellationToken ct = default);
    Task<Dictionary<RuleGroupTypes, string>> GetConditions(long clientId, CancellationToken ct = default);
    Task<Dictionary<RuleGroupTypes, string>> GetActions(long clientId, CancellationToken ct = default);
}
