using System.Text.Json.Nodes;
using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Models.Responses.Rule;

public record RuleResponse(long Id, string Name, StatusTypes Status, JsonNode? Rules);