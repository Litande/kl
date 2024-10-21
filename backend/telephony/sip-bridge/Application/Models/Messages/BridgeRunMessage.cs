namespace Plat4Me.DialSipBridge.Application.Models.Messages;

public record BridgeRunMessage(
    string BridgeId,
    string BridgeAddr)
{
    public string Initiator => nameof(DialSipBridge);
}
