using KL.Manager.API.Application.Models.Responses.Audio;

namespace KL.Manager.API.Application.Handlers;

public interface IDownloadAudioRecordHandler
{
    Task<AudioRecordResponse?> Handle(long clientId, long userId, long callId, DateTimeOffset? ifModifiedSince, CancellationToken ct = default);
}