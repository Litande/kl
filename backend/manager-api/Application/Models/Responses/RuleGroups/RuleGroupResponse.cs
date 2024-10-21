using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Application.Models.Responses.Rule;

namespace Plat4Me.DialClientApi.Application.Models.Responses.RuleGroups;

public record RuleGroupResponse(
    long Id,
    string Name,
    StatusTypes Status,
    IEnumerable<RuleResponse> Rules);