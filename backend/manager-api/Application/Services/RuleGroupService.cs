using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Application.Extensions;
using Plat4Me.DialClientApi.Application.Models.Requests.RuleGroups;
using Plat4Me.DialClientApi.Application.Models.Responses.RuleGroups;
using Plat4Me.DialClientApi.Application.Services.Interfaces;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialClientApi.Application.Services;

public class RuleGroupService : IRuleGroupService
{
    private readonly IRuleGroupRepository _ruleGroupRepository;

    public RuleGroupService(IRuleGroupRepository ruleGroupRepository)
    {
        _ruleGroupRepository = ruleGroupRepository;
    }

    public async Task<IEnumerable<RuleGroupResponse>> GetRuleGroups(
        long currentClientId,
        RuleGroupTypes ruleType,
        CancellationToken ct = default)
    {
        var ruleGroups = await _ruleGroupRepository.GetRuleGroups(currentClientId, ruleType, ct);

        var response = ruleGroups
            .Select(i => new RuleGroupResponse(
                i.Id,
                i.Name,
                i.Status,
                i.Rules.ToRulesResponse()));

        return response;
    }

    public async Task<RuleGroupResponse> AddRuleGroup(
        long currentClientId,
        CreateRuleGroupRequest request,
        RuleGroupTypes ruleType,
        CancellationToken ct = default)
    {
        var entity = request.ToModel(ruleType, currentClientId);
        var addEntity = await _ruleGroupRepository.AddRuleGroup(entity, ct);

        return addEntity.ToResponse();
    }

    public async Task<RuleGroupResponse> UpdateRuleGroup(
        long currentClientId,
        UpdateRuleGroupRequest request,
        long groupId,
        RuleGroupTypes ruleType,
        CancellationToken ct = default)
    {
        var ruleGroup = await _ruleGroupRepository.GetById(currentClientId, groupId, ct);
        if (ruleGroup is null)
            throw new KeyNotFoundException($"Cannot find rule group with id: {groupId}");

        ruleGroup = ruleGroup.ToModel(request, ruleType);
        await _ruleGroupRepository.SaveChanges(ct);

        return ruleGroup.ToResponse();
    }

    public async Task<RuleGroupResponse> UpdateRuleGroupStatus(
        long currentClientId,
        StatusTypes status,
        long groupId,
        CancellationToken ct = default)
    {
        var ruleGroup = await _ruleGroupRepository.GetById(currentClientId, groupId, ct);
        if (ruleGroup is null)
            throw new KeyNotFoundException($"Cannot find rule group with id: {groupId}");

        ruleGroup.Status = status;
        await _ruleGroupRepository.SaveChanges(ct);

        return ruleGroup.ToResponse();
    }
}
