namespace Plat4Me.DialLeadCaller.Application.Clients;

public interface IBridgeClient
{
    Task<string?> Ping(string bridgeAddr);
}
