namespace Plat4Me.DialLeadCaller.Application.Models.Messages;

public record MixedRecordReadyMessage(
    string SessionId,
    string? RecordMixedFile,
    string Initiator);
