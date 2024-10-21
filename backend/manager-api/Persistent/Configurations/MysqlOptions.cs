using System.Text.Json.Serialization;

namespace Plat4Me.DialClientApi.Persistent.Configurations;

public class MysqlOptions
{
    [JsonPropertyName("DB_HOST")]
    public string Host { get; set; }

    [JsonPropertyName("DB_PORT")]
    public string Port { get; set; }

    [JsonPropertyName("DB_USER")]
    public string User { get; set; }

    [JsonPropertyName("DB_PASS")]
    public string Pass { get; set; }

    /// <summary>
    /// The maximum number of connection retry attempts.
    /// default number is 6 times
    /// </summary>
    [JsonPropertyName("ConnectRetry")]
    public int ConnectRetry { get; set; } = 6;

    public string GetUrl(string database) => $"server={Host};port={Port};database={database};user={User};password={Pass};Convert Zero Datetime=True";
}
