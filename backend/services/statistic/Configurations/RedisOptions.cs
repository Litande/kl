using System.Text.Json.Serialization;
using StackExchange.Redis;

namespace KL.Statistics.Configurations;

public class RedisOptions
{
    [JsonPropertyName("HOST")]
    public string Host { get; set; } = null!;

    [JsonPropertyName("PORT")]
    public string Port { get; set; } = null!;

    private const int LinearRetryTimeout = 1000; //ms, 1 sec

    public ConfigurationOptions GetConfiguration() => new()
    {
        EndPoints = new EndPointCollection { $"{Host}:{Port}" },
        ReconnectRetryPolicy = new LinearRetry(LinearRetryTimeout),
    };
}
