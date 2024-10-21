namespace Plat4Me.DialSipBridge.Application.Models;


public record UploadRecordsRequest(
    string SessionId,
    string? RecordLeadFile,
    string[]? RecordUserFiles,
    string[]? RecordManagerFiles
);
