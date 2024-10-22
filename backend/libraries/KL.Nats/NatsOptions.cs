using System.Text.Json.Serialization;

namespace KL.Nats;

public class NatsOptions
{
    [JsonPropertyName("HOST")] 
    public string Host { get; set; } = null!;

    [JsonPropertyName("PORT")]
    public string Port { get; set; } = null!;

    [JsonPropertyName("RECONNECTLIMIT")]
    public int ReconnectLimit { get; set; }

    [JsonPropertyName("RECONNECTINTERVAL")]
    public int ReconnectInterval { get; set; }

    [JsonPropertyName("NATSSTREAMINGCLUSTERID")]
    public string NatsStreamingClusterId { get; set; } = null!;

    [JsonPropertyName("CLIENTID")]
    public string ClientId { get; set; } = null!;

    [JsonPropertyName("PUBACKWAIT")]
    public int PubAckWait { get; set; }

    [JsonPropertyName("CONNECTTIMEOUT")]
    public int ConnectTimeout { get; set; }

    [JsonPropertyName("PINGMAXOUTSTANDING")]
    public int PingMaxOutstanding { get; set; }

    [JsonPropertyName("PINGINTERVAL")]
    public int PingInterval { get; set; }

    [JsonPropertyName("BRANDIDTOBEMOVED")]
    public string BrandIdToBeMoved { get; set; } = null!;

    public string GetUrl() => $"nats://{Host}:{Port}";
}
