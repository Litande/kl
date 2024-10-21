namespace Plat4Me.DialSipBridge.Application.Configurations;


public class IceServer
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string Urls { get; set; } = ""; //"stun:domain:port"
}


public class RTCOptions
{
    public int ListenPort { get; set; }
    public string WebSocketPath { get; set; } = "/ws";
    public int WebSocketPingPeriod { get; set; } = 30;
    public int RTPPortRangeStart { get; set; }
    public int RTPPortRangeEnd { get; set; }
    public bool UseIceServers { get; set; } = true;
    public List<string> IceCandidates { get; set; } = new();
    public List<IceServer> IceServers { get; set; } = new();
    public string Certificate { get; set; } = null!;
    public string PrivateKey { get; set; } = null!;
}
