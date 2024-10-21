using System.Text.Json.Serialization;

namespace Plat4Me.DialSipBridge.Application.Models;

public record HangupRequest
{
    [JsonPropertyName("hangupReason")]
    public string? Reason { get; set; }
}
