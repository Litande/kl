using KL.Manager.API.Application.Enums;
using KL.Manager.API.Application.Models;
using KL.Manager.API.Application.Models.Requests.Rule;
using KL.Manager.API.Application.Models.Responses.Rule;
using KL.Manager.API.Persistent.Entities.Projections;

namespace KL.Manager.API.Application.Services.Interfaces;

public interface IRuleService
{
    Task<RuleResponse> GetRule(long clientId, long ruleId, CancellationToken ct = default);
    Task<RuleResponse> AddRule(long clientId, CreateRuleRequest request, RuleGroupTypes ruleType, long ruleGroupId, CancellationToken ct = default);
    Task<RuleResponse> UpdateRule(long clientId, UpdateRuleRequest request, long ruleId, RuleGroupTypes ruleType, long groupId, CancellationToken ct = default);
    Task<string> GetConditions(long clientId, RuleGroupTypes ruleType, CancellationToken ct = default);
    Task<string> GetActions(long clientId, RuleGroupTypes ruleType, CancellationToken ct = default);
    Task<OperationResponse> ValidateRule(long clientId, RuleGroupTypes ruleType, string rule, CancellationToken ct = default);
    Task<RuleResponse> UpdateRuleStatus(long clientId, StatusTypes status, long ruleId, CancellationToken ct = default);
    Task<IEnumerable<StatusRuleProjection>> GetStatusRules(long clientId, CancellationToken ct = default);
    Task UpdateStatusRules(long clientId, IEnumerable<StatusRuleProjection> rules, CancellationToken ct = default);
}
