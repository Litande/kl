using Plat4Me.DialAgentApi.Application.Models.Responses;

namespace Plat4Me.DialAgentApi.Application.Handlers;

public interface IDownloadAudioRecordHandler
{
    Task<AudioRecordResponse?> Handle(long clientId, long userId, long callId, DateTimeOffset? ifModifiedSince, CancellationToken ct = default);
}