using System.Net;
using KL.SIP.Bridge.Application.Configurations;
using KL.SIP.Bridge.Application.Enums;
using SIPSorcery.Net;
using SIPSorceryMedia.Abstractions;

namespace KL.SIP.Bridge.Application.Connections;

public class RTCConnection : IConnection
{
    public event Action<CallFinishReasons>? OnCallFailed;
    public event Action? OnCallAnswered;
    public event Action? OnCallHangup;
    public event Action? OnCallRinging;
    public event Action<IConnection, SDPMediaTypesEnum, RTPPacket>? OnData;

    public int? LastErrorCode => null;
    public RTCPeerConnection PeerConnection => _connection;

    protected RTCOptions _settings;
    protected RTCPeerConnection _connection;
    protected ILogger? _logger;

    public RTCConnection(RTCOptions settings, RTCConfiguration? configuration, ILogger? logger = null)
    {
        _settings = settings;
        _logger = logger;
        _connection = new RTCPeerConnection(configuration, 0, new SIPSorcery.Sys.PortRange(_settings.RTPPortRangeStart, _settings.RTPPortRangeEnd, true));
        _connection.sctp.Close();//HACK?: force close - Firefox don't support RTCSctpTransport, as result - inf.loop
        foreach (var candidate in _settings.IceCandidates)
        {
            var rtpPort = _connection.GetRtpChannel().RTPPort;
            if (!IPAddress.TryParse(candidate, out var ipAddress))
            {
                try
                {
                    ipAddress = Dns.GetHostEntry(candidate).AddressList[0];
                }
                catch (Exception)
                {
                    ipAddress = null;
                }
            }
            if (ipAddress is null)
            {
                continue;
            }
            var publicIPv4Candidate = new RTCIceCandidate(RTCIceProtocol.udp, ipAddress, (ushort)rtpPort, RTCIceCandidateType.host);
            _connection.addLocalIceCandidate(publicIPv4Candidate);
        }
        AddAudioTrack();// main stream
        AddAudioTrack();// manager stream
        _connection.OnClosed += () =>
        {
            _logger?.LogDebug("RTC OnClose");
            OnCallHangup?.Invoke();
        };
        _connection.OnTimeout += (type) =>
        {
            _logger?.LogDebug("RTC OnTimeout");
            OnCallFailed?.Invoke(CallFinishReasons.RTCTransportTimeout);
        };
        _connection.OnRtpPacketReceived += (IPEndPoint endPoint, SDPMediaTypesEnum mediaType, RTPPacket packet) =>
        {
            OnData?.Invoke(this, mediaType, packet);
        };
    }

    protected void AddAudioTrack()
    {
        var track = new MediaStreamTrack(SDPMediaTypesEnum.audio, false,
            new List<SDPAudioVideoMediaFormat> {
                        new SDPAudioVideoMediaFormat(SDPWellKnownMediaFormatsEnum.PCMA)
                });
        _connection.addTrack(track);
    }

    public Task Start()
    {
        //_connection.Start();
        return Task.CompletedTask;
    }

    public void SendToAudioStream(int audioStreamIdx, RTPPacket packet)
    {
        if (audioStreamIdx >= 0 && audioStreamIdx < _connection.AudioStreamList.Count)
            _connection.AudioStreamList[audioStreamIdx].SendAudio((uint)packet.Payload.Length, packet.Payload);
    }

    public void Hangup()
    {
        _connection.close();
    }

    public void Dispose()
    {
        //_connection.Dispose(); // same as Close("disposed");
    }
}
