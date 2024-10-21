namespace Plat4Me.DialSipBridge.Application.Configurations;
public class SIPOptions
{
    public string ExternalIP { get; set; } = "";
    public int RTPPortRangeStart { get; set; }
    public int RTPPortRangeEnd { get; set; }
    public int UDPTransportPort { get; set; }

    public Dictionary<string, string> ProviderSecrets { get; set; } = new();
}
