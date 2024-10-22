using KL.Manager.API.Application.Enums;
using KL.Manager.API.Application.Models.Requests.RuleGroups;
using KL.Manager.API.Application.Models.Responses.RuleGroups;

namespace KL.Manager.API.Application.Services.Interfaces;

public interface IRuleGroupService
{
    Task<IEnumerable<RuleGroupResponse>> GetRuleGroups(long currentClientId, RuleGroupTypes ruleType, CancellationToken ct = default);
    Task<RuleGroupResponse> AddRuleGroup(long currentClientId, CreateRuleGroupRequest request, RuleGroupTypes ruleType, CancellationToken ct = default);
    Task<RuleGroupResponse> UpdateRuleGroup(long currentClientId, UpdateRuleGroupRequest request, long groupId, RuleGroupTypes ruleType, CancellationToken ct = default);
    Task<RuleGroupResponse> UpdateRuleGroupStatus(long currentClientId, StatusTypes status, long groupId, CancellationToken ct = default);
}
