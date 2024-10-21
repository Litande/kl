using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Models.Requests.RuleGroups;

public record UpdateRuleGroupRequest(
    string Name,
    StatusTypes Status);