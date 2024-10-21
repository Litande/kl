using Plat4Me.DialSipBridge.Application.Enums;

namespace Plat4Me.DialSipBridge.Application.Models.Messages;

public record HangupCallMessage(
    string SessionId,
    string Initiator,
    CallFinishReasons? ReasonCode = null,
    string? ReasonDetails = null);
