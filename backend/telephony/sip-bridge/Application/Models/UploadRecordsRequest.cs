namespace KL.SIP.Bridge.Application.Models;


public record UploadRecordsRequest(
    string SessionId,
    string? RecordLeadFile,
    string[]? RecordUserFiles,
    string[]? RecordManagerFiles
);
