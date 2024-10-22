using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models.Messages.Call;

public record HangupCallMessage(
    string SessionId,
    CallFinishReasons? ReasonCode = null,
    string? ReasonDetails = null)
{
    public string Initiator { get; set; } = nameof(DialClientApi);
};