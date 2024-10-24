﻿using KL.Manager.API.Application.Configurations;
using KL.Manager.API.Application.Models.Responses.Audio;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using KL.Storage;
using Microsoft.Extensions.Options;

namespace KL.Manager.API.Application.Handlers;

public class DownloadAudioRecordHandler(
    IStorageService storage,
    IOptions<GeneralOptions> generalOptions,
    ICDRRepository cdrRepository,
    ILogger<DownloadAudioRecordHandler> logger)
    : IDownloadAudioRecordHandler
{
    private readonly GeneralOptions _generalOptions = generalOptions.Value;

    public async Task<AudioRecordResponse?> Handle(
        long clientId,
        long userId,
        long callId,
        DateTimeOffset? ifModifiedSince,
        CancellationToken ct = default)
    {
        var callRecord = await cdrRepository.GetById(clientId, callId, ct);

        if (callRecord is null || string.IsNullOrEmpty(callRecord.RecordMixedFile)
                               || callRecord.CallHangupAt is null)
            return null;

        //check if audio was modified
        if (ifModifiedSince.HasValue && ifModifiedSince.Value >= callRecord.CallHangupAt)
            return new AudioRecordResponse(null, false);

        try
        {
            var stream =
                await storage.Download(Path.Combine(_generalOptions.RecordingStorePrefix, callRecord.RecordMixedFile),
                    ct);

            var fileName = Path.GetFileName(callRecord.RecordMixedFile);
            return new AudioRecordResponse(
                new AudioRecord(stream, fileName, callRecord.CallHangupAt.Value),
                true);
        }
        catch (FileNotFoundException)
        {
            logger.LogInformation("File with {Id} not found", callId);
            return null;
        }
    }
}
