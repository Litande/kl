namespace KL.Call.Mixer.Models;

public record MixedRecordReadyMessage(
    string SessionId,
    string? RecordMixedFile
);
