using System.Text.Json.Nodes;
using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models.Requests.Rule;

public record UpdateRuleRequest(
    string Name,
    StatusTypes Status,
    JsonObject Rules);