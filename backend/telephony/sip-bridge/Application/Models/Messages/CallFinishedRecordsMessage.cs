namespace Plat4Me.DialSipBridge.Application.Models.Messages;


public record CallFinishedRecordsMessage(
    string SessionId,
    string? RecordLeadFile,
    string[]? RecordUserFiles,
    string[]? RecordManagerFiles)
{
    public string Initiator => nameof(DialSipBridge);
}
