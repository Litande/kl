namespace KL.Agent.API.Application.Models.Responses;

public record AudioRecordResponse(AudioRecord? AudiRecord, bool IsModified);

public record AudioRecord(Stream Stream, string FileName, DateTimeOffset LastModifiedAt)
{
    public string Format { get; set; } = "audio/ogg";
}