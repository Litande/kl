using KL.SIP.Bridge.Application.Enums;

namespace KL.SIP.Bridge.Application.Models.Messages;

public record HangupCallMessage(
    string SessionId,
    string Initiator,
    CallFinishReasons? ReasonCode = null,
    string? ReasonDetails = null);
