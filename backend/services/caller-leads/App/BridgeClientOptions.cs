namespace KL.Caller.Leads.App;

public class BridgeClientOptions
{
    public string? PingEndpoint { get; set; }
    public long PingRequestTimeout { get; set; } = 5;
}
