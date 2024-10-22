using System.Text.Json.Nodes;
using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models.Responses.Rule;

public record RuleResponse(long Id, string Name, StatusTypes Status, JsonNode? Rules);