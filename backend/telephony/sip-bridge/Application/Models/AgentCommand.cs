using System.Text.Json.Serialization;
using KL.SIP.Bridge.Application.Enums;

namespace KL.SIP.Bridge.Application.Models;

public record AgentCommand
{
    [JsonPropertyName("command")]
    public AgentCommandTypes? Command { get; set; }
    [JsonPropertyName("params")]
    public Dictionary<string, string>? Params { get; set; }
}
