using KL.Call.Mixer.App;
using KL.Call.Mixer.Models;
using KL.Call.Mixer.Services;
using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;
using Plat4Me.Core.Storage;

namespace KL.Call.Mixer;

public class MixingBackgroundService : BackgroundService
{
    private GeneralOptions _generalOptions;
    private readonly ILogger<MixingBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    private readonly NatsSubjects _natsSubjects;
    private readonly INatsPublisher _natsPublisher;
    private readonly INatsSubscriber _natsSubscriber;

    public MixingBackgroundService(IServiceProvider serviceProvider,
        IOptions<GeneralOptions> generalOptions,
        IOptions<NatsSubjects> natsSubjects,
        INatsPublisher natsPublisher,
        INatsSubscriber natsSubscriber,
        ILogger<MixingBackgroundService> logger
    )
    {
        _generalOptions = generalOptions.Value;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _natsSubjects = natsSubjects.Value;
        _natsPublisher = natsPublisher;
        _natsSubscriber = natsSubscriber;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("{Service} Starting", nameof(MixingBackgroundService));
            await _natsSubscriber.SubscribeAsync<CallFinishedRecordsMessage>(_natsSubjects.CallFinishedRecords, Handler);
            async void Handler(CallFinishedRecordsMessage message)
            {
                try
                {
                    _logger.LogInformation("Start processing for {sessionId}", message.SessionId);
                    await using var scope = _serviceProvider.CreateAsyncScope();
                    if (string.IsNullOrEmpty(message.SessionId))
                        throw new ArgumentException(nameof(message.SessionId), "Missing sessionId");

                    string? mixedRecordName = null;
                    if (message.RecordLeadFile is not null) // if answered call
                    {
                        var mixerService = scope.ServiceProvider.GetRequiredService<IAudioMixerService>();
                        var storageService = scope.ServiceProvider.GetRequiredService<IStorageService>();
                        var leadRecord = new KeyValuePair<string, Stream>(
                            message.RecordLeadFile,
                            await storageService.Download($"{_generalOptions.RecordingStorePrefix}/{message.RecordLeadFile}", stoppingToken));

                        KeyValuePair<string, Stream>[]? userRecords = null;
                        if (message.RecordUserFiles is not null)
                        {
                            userRecords = await Task.WhenAll(
                            message.RecordUserFiles.Select(async x => new KeyValuePair<string, Stream>
                            (
                                x,
                                await storageService.Download($"{_generalOptions.RecordingStorePrefix}/{x}", stoppingToken)
                            )));
                        }

                        KeyValuePair<string, Stream>[]? managerRecords = null;
                        if (message.RecordManagerFiles is not null)
                        {
                            managerRecords = await Task.WhenAll(
                            message.RecordManagerFiles.Select(async x => new KeyValuePair<string, Stream>
                            (
                                x,
                                await storageService.Download($"{_generalOptions.RecordingStorePrefix}/{x}", stoppingToken)
                            )));
                        }
                        if (userRecords is not null && userRecords.Any() ||
                            managerRecords is not null && managerRecords.Any())
                        {
                            var output = new MemoryStream();
                            mixedRecordName = await mixerService.MixCallRecords(
                                message.SessionId,
                                leadRecord,
                                userRecords,
                                managerRecords,
                                output,
                                stoppingToken
                            );
                            if (mixedRecordName is not null)
                            {
                                output.Position = 0;
                                await storageService.Upload($"{_generalOptions.RecordingStorePrefix}/{mixedRecordName}", mixerService.ContentType, output, true, stoppingToken);
                            }
                        }
                        _logger.LogDebug("Publish MixedRecordReady Session={session} MixedRecord={recordFile}",
                              message.SessionId, mixedRecordName);
                        await _natsPublisher.PublishAsync(_natsSubjects.MixedRecordReady,
                              new MixedRecordReadyMessage(
                                  message.SessionId,
                                  mixedRecordName
                          ));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during CallFinishedRecordsMessage handler executing");
                }
            }

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during {Service} executing", nameof(MixingBackgroundService));
            //throw;
        }
    }
}
