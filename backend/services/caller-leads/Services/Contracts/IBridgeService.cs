namespace Plat4Me.DialLeadCaller.Application.Services.Contracts;

public interface IBridgeService
{
    void RegisterBridge(string bridgeId, string bridgeAddr);
    bool AnyBridgeRegistered();
    string? GetBridge();
    Task PingBridges();
}
