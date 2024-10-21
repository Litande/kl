namespace Plat4Me.DialLeadCaller.Application.Models;

public record BridgeRegRequestMessage
{
    public string Initiator => nameof(DialLeadCaller);
}
