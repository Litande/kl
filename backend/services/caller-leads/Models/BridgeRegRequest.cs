namespace KL.Caller.Leads.Models;

public record BridgeRegRequestMessage
{
    public string Initiator => nameof(KL.Caller);
}
