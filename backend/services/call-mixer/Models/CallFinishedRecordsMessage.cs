namespace KL.Call.Mixer.Models;


public record CallFinishedRecordsMessage(
    string SessionId,
    string? RecordLeadFile,
    string[]? RecordUserFiles,
    string[]? RecordManagerFiles
);
