using System.Text.Json.Serialization;

namespace KL.Nats;

public class NatsOptions
{
    [JsonPropertyName("HOST")]
    public string Host { get; set; }

    [JsonPropertyName("PORT")]
    public string Port { get; set; }

    [JsonPropertyName("RECONNECTLIMIT")]
    public int ReconnectLimit { get; set; }

    [JsonPropertyName("RECONNECTINTERVAL")]
    public int ReconnectInterval { get; set; }

    [JsonPropertyName("NATSSTREAMINGCLUSTERID")]
    public string NatsStreamingClusterId { get; set; }

    [JsonPropertyName("CLIENTID")]
    public string ClientId { get; set; }

    [JsonPropertyName("PUBACKWAIT")]
    public int PubAckWait { get; set; }

    [JsonPropertyName("CONNECTTIMEOUT")]
    public int ConnectTimeout { get; set; }

    [JsonPropertyName("PINGMAXOUTSTANDING")]
    public int PingMaxOutstanding { get; set; }

    [JsonPropertyName("PINGINTERVAL")]
    public int PingInterval { get; set; }

    [JsonPropertyName("BRANDIDTOBEMOVED")]
    public string BrandIdToBeMoved { get; set; }

    public string GetUrl() => $"nats://{Host}:{Port}";
}
