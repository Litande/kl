namespace Plat4Me.DialCallRecordMixer.Services;

public interface IAudioMixerService
{
    string ContentType { get; }
    Task<string?> MixCallRecords(
        string sessionId,
        KeyValuePair<string, Stream> leadRecord,
        IEnumerable<KeyValuePair<string, Stream>>? userRecords,
        IEnumerable<KeyValuePair<string, Stream>>? managerRecords,
        Stream output,
        CancellationToken ct = default
    );
}
