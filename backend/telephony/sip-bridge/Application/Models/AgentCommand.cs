using System.Text.Json.Serialization;
using Plat4Me.DialSipBridge.Application.Enums;

namespace Plat4Me.DialSipBridge.Application.Models;

public record AgentCommand
{
    [JsonPropertyName("command")]
    public AgentCommandTypes? Command { get; set; }
    [JsonPropertyName("params")]
    public Dictionary<string, string>? Params { get; set; }
}
