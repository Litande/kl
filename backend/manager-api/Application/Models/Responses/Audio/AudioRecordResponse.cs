namespace Plat4Me.DialClientApi.Application.Models.Responses.Audio;

public record AudioRecordResponse(AudioRecord? AudiRecord, bool IsModified);

public record AudioRecord(Stream Stream, string FileName, DateTimeOffset LastModifiedAt)
{
    public string Format { get; set; } = "audio/ogg";
}