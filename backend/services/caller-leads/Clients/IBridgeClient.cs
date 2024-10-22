namespace KL.Caller.Leads.Clients;

public interface IBridgeClient
{
    Task<string?> Ping(string bridgeAddr);
}
