namespace KL.SIP.Bridge.Application.Models.Messages;

public record BridgeRunMessage(
    string BridgeId,
    string BridgeAddr)
{
    public string Initiator => nameof(DialSipBridge);
}
