namespace Plat4Me.DialCallRecordMixer.Models;


public record CallFinishedRecordsMessage(
    string SessionId,
    string? RecordLeadFile,
    string[]? RecordUserFiles,
    string[]? RecordManagerFiles
);
