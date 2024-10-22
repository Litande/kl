namespace KL.Caller.Leads.Models.Messages;

public record MixedRecordReadyMessage(
    string SessionId,
    string? RecordMixedFile,
    string Initiator);
