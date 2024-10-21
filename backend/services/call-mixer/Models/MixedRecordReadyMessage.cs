namespace Plat4Me.DialCallRecordMixer.Models;

public record MixedRecordReadyMessage(
    string SessionId,
    string? RecordMixedFile
);
