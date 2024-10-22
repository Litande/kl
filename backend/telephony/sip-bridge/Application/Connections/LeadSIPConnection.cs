using System.Collections.Concurrent;
using System.Net;
using KL.SIP.Bridge.Application.Configurations;
using KL.SIP.Bridge.Application.Enums;
using KL.SIP.Bridge.Application.Models;
using SIPSorcery.Media;
using SIPSorcery.Net;
using SIPSorcery.SIP;
using SIPSorcery.SIP.App;
using SIPSorceryMedia.Abstractions;

namespace KL.SIP.Bridge.Application.Connections;

public class LeadSIPConnection : IConnection
{
    public event Action<CallFinishReasons>? OnCallFailed;
    public event Action? OnCallAnswered;
    public event Action? OnCallHangup;
    public event Action? OnCallRinging;
    public event Action<IConnection, SDPMediaTypesEnum, RTPPacket>? OnData;

    public int? LastErrorCode { get; protected set; }

    protected SIPUserAgent _sipUA;
    protected RTPSession _connection = null!;
    protected string _phoneNum;
    protected SIPOptions _settings;
    protected SipProviderInfo _provider;
    protected ILogger? _logger;



    private static readonly byte[] SilenceData = Enumerable.Repeat((byte)0x55, PCMAPacketSize).ToArray();// 0x55  corresponds "zero" level of PCMA
    protected Timer? _leadAudioStreamGenerator = null;
    protected const int PCMAPacketPeriod = 1000 / 50;
    protected const int PCMAPacketSize = 160; //8000 / 50
    protected const int DelayPacketCnt = 2;
    protected const int BufferPacketsCnt = DelayPacketCnt + 1;
    protected byte[][] _leadStreamBuffers;
    protected int _leadStreamBufferCurPacket = 0;

    protected uint? _agentLastProcessedTimestamp = null;
    protected uint? _managerLastProcessedTimestamp = null;

    protected ConcurrentQueue<RTPPacket?> _agentPackets = new();
    protected ConcurrentQueue<RTPPacket?> _managerPackets = new();

    protected bool _answered = false;

    public LeadSIPConnection(string phoneNum, SipProviderInfo provider, SIPOptions settings, SIPTransport transport, ILogger? logger = null)
    {
        _settings = settings;
        _provider = provider;
        _phoneNum = phoneNum;
        _logger = logger;

        _sipUA = CreateSipUa(transport);

        //prepare buffers
        _leadStreamBuffers = new byte[BufferPacketsCnt][];
        for (var i = 0; i < BufferPacketsCnt; ++i)
        {
            var buffer = new byte[PCMAPacketSize];
            Buffer.BlockCopy(SilenceData, 0, buffer, 0, PCMAPacketSize);
            _leadStreamBuffers[i] = buffer;
        }
    }

    private SIPUserAgent CreateSipUa(SIPTransport transport)
    {
        var sipUA = new SIPUserAgent(transport, null);

        sipUA.ClientCallFailed += (uac, error, sipResponse) =>
        {
            _logger?.LogInformation("Call with {phone} failed with '{error}'", _phoneNum, error);

            LastErrorCode = sipResponse?.StatusCode ?? 0;

            var reason = LastErrorCode switch
            {
                486 or 600 => CallFinishReasons.LeadLineBusy,
                420 => CallFinishReasons.LeadInvalidPhone,
                >= 400 and < 700 => CallFinishReasons.SIPTransportError,
                _ => CallFinishReasons.Unknown
            };
            OnCallFailed?.Invoke(reason);
        };
        sipUA.ClientCallAnswered += (ISIPClientUserAgent uac, SIPResponse sipResponse) =>
        {
            _logger?.LogInformation("Call {phoneNum} answered", _phoneNum);
            _leadAudioStreamGenerator = new Timer(LeadAudioStream, null, 0, PCMAPacketPeriod);
            _answered = true;
            OnCallAnswered?.Invoke();
        };
        sipUA.OnCallHungup += (SIPDialogue d) =>
        {
            _logger?.LogInformation("Call {phoneNum} hangup", _phoneNum);
            OnCallHangup?.Invoke();
        };
        sipUA.ClientCallRinging += (ISIPClientUserAgent uac, SIPResponse sipResponse) =>
        {
            _logger?.LogInformation("Call {phoneNum} ringing", _phoneNum);
            OnCallRinging?.Invoke();
        };

        return sipUA;
    }

    public async Task Start()
    {
        _connection = CreateRtpSession();
        if (_settings.ProviderSecrets.TryGetValue(_provider.Id.ToString(), out var secret))
        {
            await _sipUA.Call($"sip:{_phoneNum}@{_provider.ProviderAddress}", _provider.ProviderUserName, secret, _connection);
        }
    }

    private RTPSession CreateRtpSession()
    {
        RTPSession rtpCon;
        if (string.IsNullOrEmpty(_settings.ExternalIP))
            rtpCon = new RTPSession(false, false, false, null, 0,
                new SIPSorcery.Sys.PortRange(_settings.RTPPortRangeStart, _settings.RTPPortRangeEnd, true));
        else
            rtpCon = new ExternalRTPSession(IPAddress.Parse(_settings.ExternalIP), false, false, false, null,
                0, new SIPSorcery.Sys.PortRange(_settings.RTPPortRangeStart, _settings.RTPPortRangeEnd, true));

        var audioTrack = new MediaStreamTrack(SDPMediaTypesEnum.audio, false,
            new List<SDPAudioVideoMediaFormat> { new(SDPWellKnownMediaFormatsEnum.PCMA) });

        rtpCon.addTrack(audioTrack);
        rtpCon.AcceptRtpFromAny = true;

        rtpCon.OnRtpPacketReceived += (endPoint, mediaType, packet) =>
        {
            OnData?.Invoke(this, mediaType, packet);
        };

        return rtpCon;
    }

    public void Hangup()
    {
        //_logger?.LogInformation("Call hangup {}", _phoneNum);
        _leadAudioStreamGenerator?.Dispose();
        if (_sipUA.IsCalling || _sipUA.IsRinging)
            _sipUA.Cancel();
        else if (_sipUA.IsCallActive)
            _sipUA.Hangup();
    }

    public void Dispose()
    {
        _connection.Dispose();
        _connection = null!;
        _sipUA.Dispose();
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

    private void ProcessPackets(int currentPacketIdx)
    {
        var buffer = _leadStreamBuffers[currentPacketIdx % BufferPacketsCnt];
        var agentPacket = TryGetPacket(_agentPackets, ref _agentLastProcessedTimestamp);
        var managerPacket = TryGetPacket(_managerPackets, ref _managerLastProcessedTimestamp);


        if (agentPacket is not null && managerPacket is null)
        {
            Buffer.BlockCopy(agentPacket.Payload, 0, buffer, 0, Math.Min(PCMAPacketSize, agentPacket.Payload.Length));
        }
        else if (managerPacket is not null && agentPacket is null)
        {
            Buffer.BlockCopy(managerPacket.Payload, 0, buffer, 0, Math.Min(PCMAPacketSize, managerPacket.Payload.Length));
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
        _connection.SendAudio(PCMAPacketSize, buffer);
        Buffer.BlockCopy(SilenceData, 0, buffer, 0, PCMAPacketSize);//clean buffer

        ProcessPackets(_leadStreamBufferCurPacket + DelayPacketCnt);
    }

    protected void MixinAudioSamples(byte[] buffer, byte[] agentData, byte[] managerData)
    {
        var sz = Math.Min(Math.Min(PCMAPacketSize, agentData.Length), managerData.Length);
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

    protected void Normalize(byte[] buffer)
    {

    }

}
