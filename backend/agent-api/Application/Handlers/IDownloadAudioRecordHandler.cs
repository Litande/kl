using KL.Agent.API.Application.Models.Responses;

namespace KL.Agent.API.Application.Handlers;

public interface IDownloadAudioRecordHandler
{
    Task<AudioRecordResponse?> Handle(long clientId, long userId, long callId, DateTimeOffset? ifModifiedSince, CancellationToken ct = default);
}