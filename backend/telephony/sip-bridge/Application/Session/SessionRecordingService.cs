using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plat4Me.DialSipBridge.Application.Configurations;
using Plat4Me.DialSipBridge.Application.Enums;
using Plat4Me.DialSipBridge.Application.Models;
using Plat4Me.DialSipBridge.Application.Services;
using Plat4Me.DialSipBridge.Application.Services.AudioRecorders;

namespace Plat4Me.DialSipBridge.Application.Session;

public class SessionRecordingService : ISessionRecordingService
{
    private readonly IOptionsMonitor<GeneralOptions> _options;
    private readonly ILogger<SessionRecordingService> _logger;
    private readonly IUploaderService _uploaderService;

    public string _sessionId = null!;
    public string _leadPhone = null!;
    private AudioRecorder? _leadRecorder;
    private List<AudioRecorder> _agentRecorders = new List<AudioRecorder>();
    private List<AudioRecorder> _managerRecorders = new List<AudioRecorder>();

    private List<Task> _stoppingTasks = new List<Task>();


    public SessionRecordingService(
        IOptionsMonitor<GeneralOptions> options,
        IUploaderService uploaderService,
        ILogger<SessionRecordingService> logger
    )
    {
        _options = options;
        _logger = logger;
        _uploaderService = uploaderService;
    }

    public void Init(string sessionId, string leadPhone)
    {
        _sessionId = sessionId;
        _leadPhone = leadPhone;
        try
        {
            if (string.IsNullOrEmpty(_sessionId))
                throw new ArgumentNullException(nameof(sessionId), "SessionId required");
            Directory.CreateDirectory(Path.Combine(_options.CurrentValue.RecordingTemporaryDir, _sessionId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Can not create directory for session {0}", sessionId);
            sessionId = null!;
        }
    }

    public Task<IAudioStreamRecorder?> CreateLeadRecorder()
    {
        if (!_options.CurrentValue.RecordingEnabled)
            return Task.FromResult((IAudioStreamRecorder?)null);
        if (string.IsNullOrEmpty(_sessionId))
        {
            _logger.LogError("SessionId required");
            return Task.FromResult((IAudioStreamRecorder?)null);
        }
        if (_leadRecorder is not null)
        {
            _logger.LogWarning("Lead recorder already created");
            return Task.FromResult((IAudioStreamRecorder?)_leadRecorder);
        }
        var recorder = CreateAudioRecorder(_logger, _sessionId, _leadPhone, "lead", _options.CurrentValue.RecordingTemporaryDir);
        _leadRecorder = recorder;
        return Task.FromResult((IAudioStreamRecorder?)recorder);
    }

    public Task<IAudioStreamRecorder?> CreateAgentRecorder()
    {
        if (!_options.CurrentValue.RecordingEnabled)
            return Task.FromResult((IAudioStreamRecorder?)null);
        if (string.IsNullOrEmpty(_sessionId))
        {
            _logger.LogError("SessionId required");
            return Task.FromResult((IAudioStreamRecorder?)null);
        }
        var recorder = CreateAudioRecorder(_logger, _sessionId, _leadPhone, "agent", _options.CurrentValue.RecordingTemporaryDir);
        if (recorder is not null)
            _agentRecorders.Add(recorder);
        return Task.FromResult((IAudioStreamRecorder?)recorder);
    }

    public Task<IAudioStreamRecorder?> CreateManagerRecorder()
    {
        if (!_options.CurrentValue.RecordingEnabled || !_options.CurrentValue.ManagerRecordingEnabled)
            return Task.FromResult((IAudioStreamRecorder?)null);
        if (string.IsNullOrEmpty(_sessionId))
        {
            _logger.LogError("SessionId required");
            return Task.FromResult((IAudioStreamRecorder?)null);
        }
        var recorder = CreateAudioRecorder(_logger, _sessionId, _leadPhone, "manager", _options.CurrentValue.RecordingTemporaryDir);
        if (recorder is not null)
            _managerRecorders.Add(recorder);
        return Task.FromResult((IAudioStreamRecorder?)recorder);
    }

    public void StopRecording(IAudioStreamRecorder? target)
    {
        if (target is null)
            return;
        var recorder = target as AudioRecorder;
        if (recorder is null)
        {
            _logger.LogError("Expecting AudioStreamRecorder");
            return;
        }
        _stoppingTasks.Add(recorder.Stop());
    }

    public async Task WaitAllRecordings()
    {
        try
        {
            _stoppingTasks.AddRange(_agentRecorders.Where(x => x.IsRunning()).Select(x => x.Stop()));
            _stoppingTasks.AddRange(_managerRecorders.Where(x => x.IsRunning()).Select(x => x.Stop()));
            if (_leadRecorder is not null && _leadRecorder.IsRunning()) _stoppingTasks.Add(_leadRecorder.Stop());
            await Task.WhenAll(_stoppingTasks);

            _uploaderService.UploadRecords(
                new UploadRecordsRequest(
                    _sessionId,
                    _leadRecorder?.RecordName,
                    _agentRecorders.Any() ? _agentRecorders.Select(x => x.RecordName).ToArray() : null,
                    _managerRecorders.Any() ? _managerRecorders.Select(x => x.RecordName).ToArray() : null
                ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WaitAllRecordings failed");
        }
    }

    private AudioRecorder? CreateAudioRecorder(ILogger logger,
        string sessionId,
        string leadPhone,
        string endpoint,
        string outputDirectory)
    {
        switch (_options.CurrentValue.RecordingFormat)
        {
            case AudioRecordFormat.Raw:
                return new RawAudioRecorder(logger, sessionId, leadPhone, endpoint, outputDirectory);
            case AudioRecordFormat.Opus:
                return new OpusAudioRecorder(logger, sessionId, leadPhone, endpoint, outputDirectory);
            default:
            {
                _logger.LogError("Unknown recording format");
                return null;
            }
        }
    }

}
