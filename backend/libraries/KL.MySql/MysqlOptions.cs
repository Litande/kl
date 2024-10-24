using System.Text.Json.Serialization;

namespace KL.MySql;

public class MysqlOptions()
{
    [JsonPropertyName("HOST")]
    public string Host { get; set; }

    [JsonPropertyName("PORT")]
    public string Port { get; set; }
    
    [JsonPropertyName("USER")]
    public string User { get; set; }
    
    [JsonPropertyName("PASS")]
    public string Pass { get; set; }
    
    /// <summary>
    /// The maximum number of connection retry attempts, default number of attempts (3)
    /// </summary>
    [JsonPropertyName("ConnectRetry")]
    public int ConnectRetry { get; set; } = 3;

    public string GetUrl(string database) => $"server={Host};port={Port};database={database};user={User};password={Pass};Convert Zero Datetime=True";
}