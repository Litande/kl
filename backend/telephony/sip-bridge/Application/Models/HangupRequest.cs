using System.Text.Json.Serialization;

namespace KL.SIP.Bridge.Application.Models;

public record HangupRequest
{
    [JsonPropertyName("hangupReason")]
    public string? Reason { get; set; }
}
