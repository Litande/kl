using KL.Manager.API.Application.Enums;
using KL.Manager.API.Application.Models.Responses.Rule;

namespace KL.Manager.API.Application.Models.Responses.RuleGroups;

public record RuleGroupResponse(
    long Id,
    string Name,
    StatusTypes Status,
    IEnumerable<RuleResponse> Rules);