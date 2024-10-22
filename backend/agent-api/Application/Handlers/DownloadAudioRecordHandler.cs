using KL.Agent.API.Application.Configurations;
using KL.Agent.API.Application.Models.Responses;
using KL.Agent.API.Persistent.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using Plat4Me.Core.Storage;

namespace KL.Agent.API.Application.Handlers;

public class DownloadAudioRecordHandler : IDownloadAudioRecordHandler
{
    private readonly IStorageService _storage;
    private readonly GeneralOptions _generalOptions;
    private readonly ICDRRepository _cdrRepository;
    private readonly ILogger<DownloadAudioRecordHandler> _logger;

    public DownloadAudioRecordHandler(
        IStorageService storage,
        IOptions<GeneralOptions> generalOptions,
        ICDRRepository cdrRepository,
        ILogger<DownloadAudioRecordHandler> logger)
    {
        _storage = storage;
        _generalOptions = generalOptions.Value;
        _cdrRepository = cdrRepository;
        _logger = logger;
    }

    public async Task<AudioRecordResponse?> Handle(
        long clientId,
        long userId,
        long callId,
        DateTimeOffset? ifModifiedSince,
        CancellationToken ct = default)
    {
        var callRecord = await _cdrRepository.GetById(clientId, callId, ct);

        if (callRecord is null || string.IsNullOrEmpty(callRecord.RecordMixedFile)
                               || callRecord.CallHangupAt is null
                               || callRecord.UserId != userId)
            return null;

        //check if audio was modified
        if (ifModifiedSince.HasValue && ifModifiedSince.Value >= callRecord.CallHangupAt)
            return new AudioRecordResponse(null, false);

        try
        {
            var stream =
                await _storage.Download(Path.Combine(_generalOptions.RecordingStorePrefix, callRecord.RecordMixedFile),
                    ct);

            var fileName = Path.GetFileName(callRecord.RecordMixedFile);
            return new AudioRecordResponse(
                new AudioRecord(stream, fileName, callRecord.CallHangupAt.Value),
                true);
        }
        catch (FileNotFoundException)
        {
            _logger.LogInformation("File with {Id} not found", callId);
            return null;
        }
    }
}
