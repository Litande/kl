using Plat4Me.DialSipBridge.Application.Configurations;
using Plat4Me.DialSipBridge.Application.Services;
using SIPSorcery.Media;
using SIPSorcery.Net;
using System.Collections.Concurrent;
using Plat4Me.DialSipBridge.Application.Enums;

namespace Plat4Me.DialSipBridge.Application.Connections;

public class FakeSIPConnection : IConnection
{
    public event Action<CallFinishReasons>? OnCallFailed;
    public event Action? OnCallAnswered;
    public event Action? OnCallHangup;
    public event Action? OnCallRinging;
    public event Action<IConnection, SDPMediaTypesEnum, RTPPacket>? OnData;
    public int? LastErrorCode => null;

    protected string _phoneNum;
    protected FakeCallOptions _fakeCallOptions;
    protected ILogger? _logger;
    protected Timer? _leadAudioStreamGenerator = null;
    protected Timer? _connectionTick = null;

    protected const int DelayPacketCnt = 2;
    protected const int BufferPacketsCnt = DelayPacketCnt + 1;
    protected byte[][] _leadStreamBuffers;
    protected int _leadStreamBufferCurPacket = 0;

    protected uint? _agentLastProcessedTimestamp = null;
    protected uint? _managerLastProcessedTimestamp = null;

    protected ConcurrentQueue<RTPPacket?> _agentPackets = new();
    protected ConcurrentQueue<RTPPacket?> _managerPackets = new();

    protected bool _answered = false;
    protected bool _hangup = false;

    protected FileStream? _fakeData = null!;
    protected uint _fakeDataTimestamp = 0;

    public FakeSIPConnection(string phoneNum, FakeCallOptions fakeCallOptions, ILogger? logger = null)
    {
        _phoneNum = phoneNum;
        _fakeCallOptions = fakeCallOptions;
        _logger = logger;

        //prepare buffers
        _leadStreamBuffers = new byte[BufferPacketsCnt][];
        for (var i = 0; i < BufferPacketsCnt; ++i)
        {
            var buffer = new byte[IAudioStreamRecorder.PCMAPacketSamplesCount];
            Buffer.BlockCopy(IAudioStreamRecorder.SilenceData, 0, buffer, 0, IAudioStreamRecorder.PCMAPacketSamplesCount);
            _leadStreamBuffers[i] = buffer;
        }
    }

    public Task Start()
    {
        var rnd = new Random();
        var answerDelay = rnd.NextInt64(_fakeCallOptions.MinAnswerDelay, _fakeCallOptions.MaxAnswerDelay);
        _logger?.LogInformation("Start FakeCall to {phoneNum} with answer delay {delay}", _phoneNum, answerDelay);
        _connectionTick = new Timer(
            FakeCallAnswer,
            null,
            answerDelay,
            Timeout.Infinite
        );
        return Task.CompletedTask;
    }

    public void Hangup()
    {
        _hangup = true;
        _leadAudioStreamGenerator?.Dispose();
        _connectionTick?.Dispose();
        _fakeData?.Dispose();
    }

    public void Dispose()
    {

    }

    public void SendToAudioStream(int audioStreamIdx, RTPPacket? packet)
    {
        if (!_answered) return;//ignore packets while call initiating
        switch (audioStreamIdx)
        {
            case 0: //agent stream
                _agentPackets.Enqueue(packet);
                break;
            case 1: //manager stream
                _managerPackets.Enqueue(packet);
                break;
        }
    }

    protected void FakeCallAnswer(object? _)
    {
        _logger?.LogInformation("FakeCall {phoneNum} answered", _phoneNum);
        _leadAudioStreamGenerator = new Timer(LeadAudioStream, null, 0, IAudioStreamRecorder.PCMAPacketSamplesCount);
        _answered = true;
        OnCallAnswered?.Invoke();
        if (_fakeCallOptions.DemoFiles.Any())
        {
            var rnd = new Random();
            try
            {
                _fakeData = File.OpenRead(_fakeCallOptions.DemoFiles[rnd.NextInt64(_fakeCallOptions.DemoFiles.Count())]);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Can not open demofile");
                _fakeData = null;
            }
        }
        _connectionTick = new Timer(LeadDataGenerator, null, 0, 1000 / IAudioStreamRecorder.PCMAPacketsPerSecond);
    }

    protected void LeadDataGenerator(object? _)
    {
        if (_hangup) return;
        var packet = new RTPPacket();
        packet.Payload = IAudioStreamRecorder.SilenceData;
        packet.Header.PayloadSize = IAudioStreamRecorder.PCMAPacketSamplesCount;
        packet.Header.Timestamp = _fakeDataTimestamp;

        if (_fakeData is not null)
        {
            byte[]? buffer = new byte[IAudioStreamRecorder.PCMAPacketSamplesCount];
            var retry = 0;
            while (retry < 2)
            {
                try
                {
                    int bytesRead = _fakeData.Read(buffer, 0, IAudioStreamRecorder.PCMAPacketSamplesCount);
                    if (bytesRead == 0)
                    {
                        _fakeData.Seek(0, SeekOrigin.Begin);
                        ++retry;
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Demo read failed");
                    _fakeData.Dispose();
                    _fakeData = null;
                }
                break;
            }

            if (retry == 2)
            {
                _fakeData?.Dispose();
                _fakeData = null;
            }
            else
                packet.Payload = buffer;
        }
        if (!_hangup) OnData?.Invoke(this, SDPMediaTypesEnum.audio, packet);
        _fakeDataTimestamp += IAudioStreamRecorder.PCMAPacketSamplesCount;
    }

    private void ProcessPackets(int currentPacketIdx)
    {
        var buffer = _leadStreamBuffers[currentPacketIdx % BufferPacketsCnt];
        var agentPacket = TryGetPacket(_agentPackets, ref _agentLastProcessedTimestamp);
        var managerPacket = TryGetPacket(_managerPackets, ref _managerLastProcessedTimestamp);


        if (agentPacket is not null && managerPacket is null)
        {
            Buffer.BlockCopy(agentPacket.Payload, 0, buffer, 0, Math.Min(IAudioStreamRecorder.PCMAPacketSamplesCount, agentPacket.Payload.Length));
        }
        else if (managerPacket is not null && agentPacket is null)
        {
            Buffer.BlockCopy(managerPacket.Payload, 0, buffer, 0, Math.Min(IAudioStreamRecorder.PCMAPacketSamplesCount, managerPacket.Payload.Length));
        }
        else if (agentPacket is not null && managerPacket is not null)
        {
            MixinAudioSamples(buffer, agentPacket.Payload, managerPacket.Payload);
        }
    }

    private RTPPacket? TryGetPacket(ConcurrentQueue<RTPPacket?> packets, ref uint? _lastTimeStamp)
    {
        RTPPacket? packet = null;
        while (packets.TryDequeue(out packet))
        {
            if (packet is null)
            {
                _lastTimeStamp = null;
                packet = null;
                _logger?.LogWarning("Reset lastTimestamp");
                continue;
            }
            if (_lastTimeStamp.HasValue)
            {
                var tsDiff = TimestampDiff(packet.Header.Timestamp, _lastTimeStamp.Value);
                if (tsDiff > (uint)Int32.MaxValue)//ignore packet from the past
                {
                    _logger?.LogWarning("Old packet {diff} {last} {current}", tsDiff, _lastTimeStamp, packet.Header.Timestamp);
                    packet = null;
                    continue;
                }
                if (tsDiff > 8000 * 3) // sanity - new packet more than 3sec from prev, bugged or malformed
                {
                    _logger?.LogWarning("Bugged or malformed packet {diff} {last} {current}", tsDiff, _lastTimeStamp, packet.Header.Timestamp);
                    packet = null;
                    _lastTimeStamp = null;
                    continue;
                }
            }
            _lastTimeStamp = packet.Header.Timestamp;
            break;
        }
        return packet;
    }

    protected uint TimestampDiff(uint current, uint previous)
    {
        return current >= previous
                ? current - previous
                : uint.MaxValue - previous + current + 1;
    }

    private void LeadAudioStream(object? _)
    {
        var buffer = _leadStreamBuffers[_leadStreamBufferCurPacket % BufferPacketsCnt];
        ++_leadStreamBufferCurPacket;
        //do nothing here
        Buffer.BlockCopy(IAudioStreamRecorder.SilenceData, 0, buffer, 0, IAudioStreamRecorder.PCMAPacketSamplesCount);//clean buffer

        ProcessPackets(_leadStreamBufferCurPacket + DelayPacketCnt);
    }

    protected void MixinAudioSamples(byte[] buffer, byte[] agentData, byte[] managerData)
    {
        var sz = Math.Min(Math.Min(IAudioStreamRecorder.PCMAPacketSamplesCount, agentData.Length), managerData.Length);
        for (var i = 0; i < sz; ++i)
        {
            //Ref.: http://www.vttoth.com/CMS/index.php/technical-notes/68
            int a = ALawDecoder.ALawToLinearSample(agentData[i]) + 32768;//decode from ALaw and convert to unsinged representation
            int b = ALawDecoder.ALawToLinearSample(managerData[i]) + 32768;//decode from ALaw and convert to unsinged representation
            int m = 0;
            if (a < 32768 && b < 32768) //process "quite" data
                m = a * b / 32768;
            else //process "loud" data
                m = 2 * (a + b) - (a * b) / 32768 - 65536;
            m = Math.Clamp(m, 0, 65535);
            buffer[i] = ALawEncoder.LinearToALawSample((short)(m - 32768));//convert back to signed and encode to ALaw
        }
    }
}
