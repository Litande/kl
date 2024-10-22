using System.Collections.Concurrent;
using KL.Nats;
using KL.SIP.Bridge.Application.Configurations;
using KL.SIP.Bridge.Application.Models;
using KL.SIP.Bridge.Application.Models.Messages;
using KL.SIP.Bridge.Application.Services;
using KL.Storage;
using Microsoft.Extensions.Options;

namespace KL.SIP.Bridge.Application.Workers;

public class UploaderBackgroundService : BackgroundService, IUploaderService
{
    private IOptionsMonitor<GeneralOptions> _generalOptions;
    private readonly ILogger<UploaderBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly NatsSubjects _natsSubjects;
    private readonly INatsPublisher _natsPublisher;

    private ConcurrentQueue<UploadRecordsRequest> _requests = new();

    public UploaderBackgroundService(IServiceProvider serviceProvider,
        IOptionsMonitor<GeneralOptions> generalOptions,
        IOptions<NatsSubjects> natsSubjects,
        INatsPublisher natsPublisher,
        ILogger<UploaderBackgroundService> logger
    )
    {
        _generalOptions = generalOptions;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _natsSubjects = natsSubjects.Value;
        _natsPublisher = natsPublisher;
    }

    public void UploadRecords(UploadRecordsRequest req)
    {
        _requests.Enqueue(req);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ScanTemporaryDir();
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_requests.TryDequeue(out var request))
            {
                await Task.Delay(1000);
                continue;
            }

            try
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                var storageService = scope.ServiceProvider.GetRequiredService<IStorageService>();

                List<string> records = new List<string>();


                string? recordLeadFile = null;
                IEnumerable<string>? recordUserFiles = null;
                IEnumerable<string>? recordManagerFiles = null;

                if (!string.IsNullOrEmpty(request.RecordLeadFile))
                {
                    recordLeadFile = request.RecordLeadFile;
                    records.Add(request.RecordLeadFile);
                }
                if (request.RecordUserFiles is not null)
                {
                    recordUserFiles = request.RecordUserFiles.Where(x => !string.IsNullOrEmpty(x));
                    if (recordUserFiles.Any())
                        records.AddRange(recordUserFiles);
                    else
                        recordUserFiles = null;
                }
                if (request.RecordManagerFiles is not null)
                {
                    recordManagerFiles = request.RecordManagerFiles.Where(x => !string.IsNullOrEmpty(x));
                      if (recordManagerFiles.Any())
                        records.AddRange(recordManagerFiles);
                    else
                        recordManagerFiles = null;
                }

                foreach (var record in records)
                {
                    using (var fstream = new FileStream(Path.Combine(_generalOptions.CurrentValue.RecordingTemporaryDir, record), FileMode.Open))
                    {
                        await storageService.Upload($"{_generalOptions.CurrentValue.RecordingStorePrefix}/{record}",  "application/octet-stream", fstream, true, new CancellationToken());
                    }
                }

                CleanupRecords(request.SessionId);
                _logger.LogInformation("Publish CallFinishedRecordsMessage {sessionId}", request.SessionId);
                await _natsPublisher.PublishAsync(_natsSubjects.CallFinishedRecords,
                    new CallFinishedRecordsMessage(
                        request.SessionId,
                        recordLeadFile,
                        recordUserFiles?.ToArray(),
                        recordManagerFiles?.ToArray()
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while uploading records");
                //_requests.Enqueue(request); //TODO rewrite, add Polly Policy
            }
        }
    }

    private void ScanTemporaryDir()
    {
        var di = new DirectoryInfo(_generalOptions.CurrentValue.RecordingTemporaryDir);
        foreach (DirectoryInfo session in di.GetDirectories())
        {
            var records = session.GetFiles();

            var leadRecord = records.FirstOrDefault(x => x.Name.Contains("_lead"))?.Name;
            var userRecords = records.Where(x => x.Name.Contains("_agent"))
                .Select(x => Path.Combine(session.Name, x.Name))
                .ToArray();
            var managerRecords = records.Where(x => x.Name.Contains("_manager"))
                .Select(x => Path.Combine(session.Name, x.Name))
                .ToArray();

            _requests.Enqueue(
                new UploadRecordsRequest(
                    session.Name,
                    leadRecord is not null ? Path.Combine(session.Name, leadRecord) : null,
                    userRecords.Any() ? userRecords : null,
                    managerRecords.Any() ? managerRecords : null                   
            ));
        }
    }

    private void CleanupRecords(string sessionId)
    {
        var sessionDir = Path.Combine(_generalOptions.CurrentValue.RecordingTemporaryDir, sessionId);
        var di = new DirectoryInfo(sessionDir);
        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }
        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete(true);
        }
        Directory.Delete(sessionDir);
    }
}
