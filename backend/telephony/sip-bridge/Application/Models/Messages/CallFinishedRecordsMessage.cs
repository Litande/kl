namespace KL.SIP.Bridge.Application.Models.Messages;


public record CallFinishedRecordsMessage(
    string SessionId,
    string? RecordLeadFile,
    string[]? RecordUserFiles,
    string[]? RecordManagerFiles)
{
    public string Initiator => nameof(KL.SIP.Bridge);
}
