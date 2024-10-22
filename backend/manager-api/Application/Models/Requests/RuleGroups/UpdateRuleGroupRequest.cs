using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models.Requests.RuleGroups;

public record UpdateRuleGroupRequest(
    string Name,
    StatusTypes Status);