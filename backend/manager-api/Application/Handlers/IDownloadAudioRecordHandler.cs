using Plat4Me.DialClientApi.Application.Models.Responses.Audio;

namespace Plat4Me.DialClientApi.Application.Handlers;

public interface IDownloadAudioRecordHandler
{
    Task<AudioRecordResponse?> Handle(long clientId, long userId, long callId, DateTimeOffset? ifModifiedSince, CancellationToken ct = default);
}