using System.Text.Json.Nodes;
using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Models.Requests.Rule;

public record CreateRuleRequest(
    string Name, 
    StatusTypes Status, 
    JsonObject Rules);