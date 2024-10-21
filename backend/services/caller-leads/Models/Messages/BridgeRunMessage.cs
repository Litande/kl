namespace Plat4Me.DialLeadCaller.Application.Models.Messages;

public record BridgeRunMessage(
    string BridgeId,
    string BridgeAddr,
    string Initiator);
