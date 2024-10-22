using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models.Requests.RuleGroups;

public record CreateRuleGroupRequest(
    string Name,
    StatusTypes Status);