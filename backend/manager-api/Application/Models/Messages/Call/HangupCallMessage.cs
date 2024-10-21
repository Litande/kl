using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Models.Messages.Call;

public record HangupCallMessage(
    string SessionId,
    CallFinishReasons? ReasonCode = null,
    string? ReasonDetails = null)
{
    public string Initiator { get; set; } = nameof(DialClientApi);
};