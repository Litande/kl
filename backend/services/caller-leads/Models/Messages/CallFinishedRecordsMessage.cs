namespace Plat4Me.DialLeadCaller.Application.Models.Messages;

public record CallFinishedRecordsMessage(
    string SessionId,
    string? RecordLeadFile,
    string[]? RecordUserFiles,
    string? RecordMixedFile,
    string[]? RecordManagerFiles,
    string Initiator);
