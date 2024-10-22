namespace KL.Caller.Leads.Models.Messages;

public record BridgeRunMessage(
    string BridgeId,
    string BridgeAddr,
    string Initiator);
