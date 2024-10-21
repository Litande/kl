using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Application.Extensions;
using Plat4Me.DialClientApi.Application.Models;
using Plat4Me.DialClientApi.Application.Models.Requests.Rule;
using Plat4Me.DialClientApi.Application.Models.Responses.Rule;
using Plat4Me.DialClientApi.Application.Services.Interfaces;
using Plat4Me.DialClientApi.Persistent.Clients;
using Plat4Me.DialClientApi.Persistent.Entities.Projections;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialClientApi.Application.Services;

public class RuleService : IRuleService
{
    private readonly IRuleRepository _ruleRepository;
    private readonly IRuleEngineClient _ruleEngineClient;
    private readonly IRuleEngineCacheRepository _ruleCacheRepository;
    private readonly IRuleGroupRepository _ruleGroupRepository;

    public RuleService(
        IRuleRepository ruleRepository,
        IRuleEngineCacheRepository ruleEngineCacheRepository,
        IRuleEngineClient ruleEngineClient,
        IRuleGroupRepository ruleGroupRepository
    )
    {
        _ruleRepository = ruleRepository;
        _ruleCacheRepository = ruleEngineCacheRepository;
        _ruleEngineClient = ruleEngineClient;
        _ruleGroupRepository = ruleGroupRepository;
    }

    public async Task<RuleResponse> GetRule(
        long clientId,
        long ruleId,
        CancellationToken ct = default)
    {
        var rule = await _ruleRepository.GetById(clientId, ruleId, ct);
        if (rule is null)
            throw new KeyNotFoundException($"Cannot find rule with id: {ruleId}");

        return rule.ToResponse();
    }

    public async Task<RuleResponse> AddRule(
        long clientId,
        CreateRuleRequest request,
        RuleGroupTypes ruleType,
        long ruleGroupId,
        CancellationToken ct = default)
    {
        var validationResult = await _ruleEngineClient.ValidateRules(clientId, ruleType, request.Rules.ToString(), ct);
        if (validationResult is null || !validationResult.Success)
        {
            throw new ArgumentException(validationResult?.Error ?? "Rule Validation failed");
        }

        var ruleGroup = await _ruleGroupRepository.GetById(clientId, ruleGroupId, ct);
        if (ruleGroup is null)
            throw new KeyNotFoundException($"Cannot find ruleGroup with id: {ruleGroupId}");

        var model = request.ToModel(ruleGroup.Id);
        var addEntity = await _ruleRepository.AddRule(model, ct);

        return addEntity.ToResponse();
    }

    public async Task<RuleResponse> UpdateRule(
        long clientId,
        UpdateRuleRequest request,
        long ruleId,
        RuleGroupTypes ruleType,
        long groupId,
        CancellationToken ct = default)
    {
        var rule = await _ruleRepository.GetById(clientId, ruleId, ct);
        if (rule is null)
            throw new KeyNotFoundException($"Cannot find rule with id: {ruleId}");

        var validationResult = await _ruleEngineClient.ValidateRules(clientId, ruleType, request.Rules.ToString(), ct);
        if (validationResult is null || !validationResult.Success)
        {
            throw new ArgumentException(validationResult?.Error ?? "Rule Validation failed");
        }

        rule = rule.ToModel(request, groupId);
        await _ruleRepository.SaveChanges(ct);

        return rule.ToResponse();
    }


    public async Task<OperationResponse> ValidateRule(
        long clientId,
        RuleGroupTypes ruleType,
        string rule,
        CancellationToken ct = default)
    {
        var validationResult = await _ruleEngineClient.ValidateRules(clientId, ruleType, rule, ct);
        if (validationResult is null)
            throw new ArgumentException("Rule Validation failed");

        return validationResult;
    }

    public async Task<RuleResponse> UpdateRuleStatus(
        long clientId,
        StatusTypes status,
        long ruleId,
        CancellationToken ct = default)
    {
        var rule = await _ruleRepository.GetById(clientId, ruleId, ct);
        if (rule is null)
            throw new KeyNotFoundException($"Cannot find rule with id: {ruleId}");

        rule.Status = status;
        await _ruleRepository.SaveChanges(ct);

        return rule.ToResponse();
    }

    public async Task<string> GetConditions(
        long clientId,
        RuleGroupTypes ruleType,
        CancellationToken ct = default)
    {
        var conditions = await _ruleCacheRepository.GetConditions(clientId, ruleType, ct);
        return conditions;
    }

    public async Task<string> GetActions(
        long clientId,
        RuleGroupTypes ruleType,
        CancellationToken ct = default)
    {
        var actions = await _ruleCacheRepository.GetActions(clientId, ruleType, ct);
        return actions;
    }

    public async Task<IEnumerable<StatusRuleProjection>> GetStatusRules(
        long clientId,
        CancellationToken ct = default)
    {
        var rules = await _ruleRepository.GetStatusRules(clientId, ct);
        return rules;
    }

    public async Task UpdateStatusRules(
        long clientId,
        IEnumerable<StatusRuleProjection> rules,
        CancellationToken ct = default)
    {
        await _ruleRepository.UpdateStatusRules(clientId, rules, ct);
    }
}
