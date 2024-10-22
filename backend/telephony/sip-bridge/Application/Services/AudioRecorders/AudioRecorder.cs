using System.Collections.Concurrent;
using SIPSorcery.Net;

namespace KL.SIP.Bridge.Application.Services.AudioRecorders;

public abstract class AudioRecorder : IAudioStreamRecorder
{
    public string RecordName { get; private set; } = null!;
    public string LocalFileName { get; private set; } = null!;
    protected abstract string FileExtension {get;}

    protected const string RecordDateTimeFormat = "yyyyMMdd'T'HHmmss.fff";
    protected int ProcessingDelay = 100;

    protected ConcurrentQueue<RTPPacket> _packets = new();
    protected readonly ILogger _logger;
    protected bool _running = true;
    protected Task _process = null!;

    protected string _sessionId = null!;
    protected string _leadPhone = null!;
    protected string _endpoint = null!;
    protected string _outputDirectory = null!;

    public AudioRecorder(
        ILogger logger,
        string sessionId,
        string leadPhone,
        string endpoint,
        string outputDirectory
    )
    {
        _logger = logger;
        _sessionId = sessionId;
        _leadPhone = leadPhone;
        _endpoint = endpoint;
        _outputDirectory = outputDirectory;
    }

    public bool IsRunning()
    {
        return _running;
    }

    public void Dispose()
    {
        if (_running)
            Task.WaitAll(Stop());
    }

    protected virtual void StartRecording()
    {
        var recordFileName = $"{DateTimeOffset.UtcNow.ToString(RecordDateTimeFormat)}_{_leadPhone}_{_endpoint}{FileExtension}";
        RecordName = Path.Combine(_sessionId, recordFileName);
        LocalFileName = Path.Combine(_outputDirectory, RecordName);
        _process = Task.Run(async () =>
        {
            try
            {
                InitOutputFile();
                await ProcessPackets();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during {Service} execution", nameof(AudioRecorder));
            }
        });
    }

    public void EnqueuePacket(RTPPacket packet)
    {
        if (!_running)
            return;
        if (_process is null)//start recording on first packet
            StartRecording();
        _packets.Enqueue(packet);
    }

    public async Task Stop()
    {
        _running = false;
        if (_process is not null)
        {
            await _process;
            _process = null!;
            CloseOutputFile();
        }
    }

    protected abstract  void InitOutputFile();
    protected abstract void CloseOutputFile();

    protected virtual async Task ProcessPackets()
    {
        uint? lastTimeStamp = null;
        while (true)
        {
            RTPPacket? packet = null;
            if (!_packets.TryDequeue(out packet))
            {
                if (_running)
                {
                    await Task.Delay(ProcessingDelay);
                    continue;
                }
                else
                {
                    break;
                }
            }

            if (lastTimeStamp is null)
                lastTimeStamp = packet.Header.Timestamp;
            uint tsDiff = TimestampDiff(packet.Header.Timestamp, lastTimeStamp.Value);
            if (tsDiff > (uint)Int32.MaxValue)//ignore packet from the past
            {
                _logger.LogDebug("Old packet {diff} {last} {current} {recordname}", tsDiff, lastTimeStamp, packet.Header.Timestamp, RecordName);
                continue;
            }
            if (tsDiff > 8000 * 10) // sanity - new packet more than 10sec from prev, bugged or malformed
            {
                _logger.LogWarning("Bugged or malformed packet {diff} {last} {current} {recordname}", tsDiff, lastTimeStamp, packet.Header.Timestamp, RecordName);
                break;
            }

            while (tsDiff > 0)
            {
                uint sz = tsDiff;
                if (sz > IAudioStreamRecorder.PCMAPacketSamplesCount)
                    sz = IAudioStreamRecorder.PCMAPacketSamplesCount;
                await SaveAudioData(IAudioStreamRecorder.SilenceData, (int)sz);
                lastTimeStamp += sz;
                tsDiff = TimestampDiff(packet.Header.Timestamp, lastTimeStamp.Value);
            }
            await SaveAudioData(packet.Payload, packet.Payload.Length);
            lastTimeStamp = packet.Header.Timestamp + (uint)packet.Payload.Length;
        }
    }

    protected abstract Task SaveAudioData(byte[] samples, int length);
    
    protected uint TimestampDiff(uint current, uint previous)
    {
        return current >= previous
                ? current - previous
                : uint.MaxValue - previous + current + 1;
    }
}
