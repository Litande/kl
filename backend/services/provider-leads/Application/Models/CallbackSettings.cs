using System.Text.Json.Serialization;

namespace KL.Provider.Leads.Application.Models;

public class CallbackSettings
{
    [JsonPropertyName("callbackType")]
    public string? CallbackType { get; set; }

    [JsonPropertyName("body")]
    public Dictionary<string, string>? Body { get; set; }

    [JsonPropertyName("query")]
    public Dictionary<string, string>? Query { get; set; }

    [JsonPropertyName("method")]
    public string Method { get; set; } = null!;

    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;

    [JsonPropertyName("headers")]
    public Dictionary<string, string>? Headers { get; set; }
}